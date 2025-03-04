using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using UrlShortener.Models;
using UrlShortener.Services;
using UrlShortener.Services.Interfaces;
using UrlShortner.Repositories;
using System;
using FluentAssertions;

namespace UrlShortener.Tests.Services
{
    public class UrlShortenerServiceTests
    {
        private readonly Mock<IUrlRepository> _urlRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILogger<UrlShortenerService>> _loggerMock;
        private readonly UrlShortenerService _urlShortenerService;

        public UrlShortenerServiceTests()
        {
            _urlRepositoryMock = new Mock<IUrlRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<UrlShortenerService>>();
            _urlShortenerService = new UrlShortenerService(_urlRepositoryMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
        }

        #region ShortenUrl Tests

        [Fact]
        public void ShortenUrl_Should_Generate_New_ShortId_When_OriginalUrl_Is_Not_In_Repository_Or_Cache()
        {
            // Arrange
            var originalUrl = "https://example.com";
            var generatedShortId = "shortid123";
            _urlRepositoryMock.Setup(r => r.GetShortIdByOriginalUrl(originalUrl)).Returns((string)null);
            _urlRepositoryMock.Setup(r => r.AddUrl(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = _urlShortenerService.ShortenUrl(originalUrl);

            // Assert
            result.Should().NotBeNull();
            result.ShortUrl.Should().StartWith("http://");
            _urlRepositoryMock.Verify(r => r.AddUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShortenUrl_Should_Return_Existing_ShortUrl_When_OriginalUrl_Is_Already_Shortened()
        {
            // Arrange
            var originalUrl = "https://example.com";
            var existingShortId = "existingid123";
            _urlRepositoryMock.Setup(r => r.GetShortIdByOriginalUrl(originalUrl)).Returns(existingShortId);

            // Act
            var result = _urlShortenerService.ShortenUrl(originalUrl);

            // Assert
            result.Should().NotBeNull();
            result.ShortUrl.Should().Be($"http://{AppDomain.CurrentDomain.FriendlyName}/{existingShortId}");
            result.ShortId.Should().Be(existingShortId);
            _urlRepositoryMock.Verify(r => r.AddUrl(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ShortenUrl_Should_Throw_Exception_When_Error_Occurs()
        {
            // Arrange
            var originalUrl = "https://example.com";
            _urlRepositoryMock.Setup(r => r.GetShortIdByOriginalUrl(originalUrl)).Throws(new Exception("Error in repository"));

            // Act
            Action act = () => _urlShortenerService.ShortenUrl(originalUrl);

            // Assert
            act.Should().Throw<Exception>().WithMessage("Error in repository");
        }

        #endregion

        #region ResolveUrl Tests

        [Fact]
        public void ResolveUrl_Should_Return_OriginalUrl_When_ShortId_Is_Found_In_Cache()
        {
            // Arrange
            var shortId = "shortid123";
            var originalUrl = "https://example.com";
            _cacheServiceMock.Setup(c => c.Get(shortId)).Returns(originalUrl);

            // Act
            var result = _urlShortenerService.ResolveUrl(shortId);

            // Assert
            result.Should().Be(originalUrl);
            _urlRepositoryMock.Verify(r => r.GetUrlByShortId(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void ResolveUrl_Should_Return_OriginalUrl_From_Repository_When_ShortId_Is_Not_Found_In_Cache()
        {
            // Arrange
            var shortId = "shortid123";
            var originalUrl = "https://example.com";
            _cacheServiceMock.Setup(c => c.Get(shortId)).Returns((string)null);
            _urlRepositoryMock.Setup(r => r.GetUrlByShortId(shortId)).Returns(originalUrl);
            _cacheServiceMock.Setup(c => c.Set(shortId, originalUrl)).Verifiable();

            // Act
            var result = _urlShortenerService.ResolveUrl(shortId);

            // Assert
            result.Should().Be(originalUrl);
            _urlRepositoryMock.Verify(r => r.GetUrlByShortId(shortId), Times.Once);
            _cacheServiceMock.Verify(c => c.Set(shortId, originalUrl), Times.Once);
        }

        [Fact]
        public void ResolveUrl_Should_Return_Null_When_ShortId_Is_Not_Found()
        {
            // Arrange
            var shortId = "nonexistentid";
            _cacheServiceMock.Setup(c => c.Get(shortId)).Returns((string)null);
            _urlRepositoryMock.Setup(r => r.GetUrlByShortId(shortId)).Returns((string)null);

            // Act
            var result = _urlShortenerService.ResolveUrl(shortId);

            // Assert
            result.Should().BeNull();
            _urlRepositoryMock.Verify(r => r.GetUrlByShortId(shortId), Times.Once);
        }

        [Fact]
        public void ResolveUrl_Should_Throw_Exception_When_Error_Occurs()
        {
            // Arrange
            var shortId = "shortid123";
            _cacheServiceMock.Setup(c => c.Get(shortId)).Throws(new Exception("Error in cache"));

            // Act
            Action act = () => _urlShortenerService.ResolveUrl(shortId);

            // Assert
            act.Should().Throw<Exception>().WithMessage("Error in cache");
        }

        #endregion
    }
}
