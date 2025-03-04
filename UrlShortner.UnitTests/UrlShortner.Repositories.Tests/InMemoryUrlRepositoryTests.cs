using Moq;
using Xunit;
using UrlShortner.Repositories;
using Microsoft.Extensions.Logging;
using System;
using FluentAssertions;

namespace UrlShortner.Tests.Repositories
{
    public class InMemoryUrlRepositoryTests
    {
        private readonly Mock<ILogger<InMemoryUrlRepository>> _loggerMock;
        private readonly InMemoryUrlRepository _urlRepository;

        public InMemoryUrlRepositoryTests()
        {
            // Initialize Mock for ILogger
            _loggerMock = new Mock<ILogger<InMemoryUrlRepository>>();

            // Create instance of InMemoryUrlRepository with mocked ILogger
            _urlRepository = new InMemoryUrlRepository(_loggerMock.Object);
        }

        // Add valid URL mapping
        [Fact]
        public void AddUrl_Should_Add_Url_Mapping_Successfully()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";

            // Act
            _urlRepository.AddUrl(shortId, originalUrl);

            // Assert
            var retrievedOriginalUrl = _urlRepository.GetUrlByShortId(shortId);
            retrievedOriginalUrl.Should().Be(originalUrl);
        }

        // Throw ArgumentException when shortId is empty
        [Fact]
        public void AddUrl_Should_Throw_ArgumentException_When_ShortId_Is_Empty()
        {
            // Arrange
            var shortId = string.Empty;
            var originalUrl = "https://www.example.com";

            // Act & Assert
            Action act = () => _urlRepository.AddUrl(shortId, originalUrl);
            act.Should().Throw<ArgumentException>();
        }

        // Throw ArgumentException when originalUrl is empty
        [Fact]
        public void AddUrl_Should_Throw_ArgumentException_When_OriginalUrl_Is_Empty()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = string.Empty;

            // Act & Assert
            Action act = () => _urlRepository.AddUrl(shortId, originalUrl);
            act.Should().Throw<ArgumentException>();
        }

        // Throw InvalidOperationException when shortId already exists
        [Fact]
        public void AddUrl_Should_Throw_InvalidOperationException_When_ShortId_Already_Exists()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";

            // Add the first mapping
            _urlRepository.AddUrl(shortId, originalUrl);

            // Act & Assert
            Action act = () => _urlRepository.AddUrl(shortId, "https://www.anotherurl.com");
            act.Should().Throw<InvalidOperationException>().WithMessage($"Short ID '{shortId}' already exists.");
        }

        // Throw InvalidOperationException when originalUrl already exists
        [Fact]
        public void AddUrl_Should_Throw_InvalidOperationException_When_OriginalUrl_Already_Exists()
        {
            // Arrange
            var shortId1 = "abc123";
            var shortId2 = "def456";
            var originalUrl = "https://www.example.com";

            // Add the first mapping
            _urlRepository.AddUrl(shortId1, originalUrl);

            // Act & Assert
            Action act = () => _urlRepository.AddUrl(shortId2, originalUrl);
            act.Should().Throw<InvalidOperationException>().WithMessage($"The original URL '{originalUrl}' is already shortened.");
        }

        // Return correct original URL for valid short ID
        [Fact]
        public void GetUrlByShortId_Should_Return_OriginalUrl_When_ShortId_Exists()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";
            _urlRepository.AddUrl(shortId, originalUrl);

            // Act
            var retrievedOriginalUrl = _urlRepository.GetUrlByShortId(shortId);

            // Assert
            retrievedOriginalUrl.Should().Be(originalUrl);
        }

        // Return null when short ID does not exist
        [Fact]
        public void GetUrlByShortId_Should_Return_Null_When_ShortId_Does_Not_Exist()
        {
            // Arrange
            var shortId = "abc123";

            // Act
            var retrievedOriginalUrl = _urlRepository.GetUrlByShortId(shortId);

            // Assert
            retrievedOriginalUrl.Should().BeNull();
        }

        // Return correct short ID for valid original URL
        [Fact]
        public void GetShortIdByOriginalUrl_Should_Return_ShortId_When_OriginalUrl_Exists()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";
            _urlRepository.AddUrl(shortId, originalUrl);

            // Act
            var retrievedShortId = _urlRepository.GetShortIdByOriginalUrl(originalUrl);

            // Assert
            retrievedShortId.Should().Be(shortId);
        }

        // Return null when original URL does not exist
        [Fact]
        public void GetShortIdByOriginalUrl_Should_Return_Null_When_OriginalUrl_Does_Not_Exist()
        {
            // Arrange
            var originalUrl = "https://www.example.com";

            // Act
            var retrievedShortId = _urlRepository.GetShortIdByOriginalUrl(originalUrl);

            // Assert
            retrievedShortId.Should().BeNull();
        }
    }
}
