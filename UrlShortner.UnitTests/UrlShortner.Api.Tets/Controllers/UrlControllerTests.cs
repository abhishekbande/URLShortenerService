using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.Models;
using UrlShortener.Services.Interfaces;
using UrlShortner.Api.Controllers;

namespace UrlShortner.UnitTests.UrlShortner.Api.Tets.Controllers
{
    public class UrlControllerTests
    {
        private readonly Mock<IUrlShortenerService> _urlShortenerServiceMock;
        private readonly Mock<ILogger<UrlController>> _loggerMock;
        private readonly UrlController _controller;
        private readonly IFixture _fixture;

        public UrlControllerTests()
        {
            _urlShortenerServiceMock = new Mock<IUrlShortenerService>();
            _loggerMock = new Mock<ILogger<UrlController>>();
            _controller = new UrlController(_urlShortenerServiceMock.Object, _loggerMock.Object);
            _fixture = new Fixture();
        }

        // Test for successful URL shortening
        [Fact]
        public void ShortenUrl_ReturnsOk_WhenUrlIsValid()
        {
            // Arrange
            var request = _fixture.Create<ShortenUrlRequest>();  // Generates a random ShortenUrlRequest
            request.OriginalUrl = "http://example.com";
            var shortUrlResponse = new ShortenUrlResponse { ShortUrl = "http://short.url/abc123" };
            _urlShortenerServiceMock.Setup(service => service.ShortenUrl(request.OriginalUrl)).Returns(shortUrlResponse);

            // Act
            var result = _controller.ShortenUrl(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ShortenUrlResponse>(okResult.Value);
            Assert.Equal(shortUrlResponse.ShortUrl, response.ShortUrl);
        }

        // Test for invalid URL (empty or null)
        [Fact]
        public void ShortenUrl_ReturnsBadRequest_WhenUrlIsEmpty()
        {
            // Arrange
            var request = _fixture.Create<ShortenUrlRequest>();
            request.OriginalUrl = ""; // Set URL to empty for this test

            // Act
            var result = _controller.ShortenUrl(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("URL cannot be empty.", badRequestResult.Value);
        }

        // Test for already shortened URL
        [Fact]
        public void ShortenUrl_ReturnsConflict_WhenUrlAlreadyShortened()
        {
            // Arrange
            var request = _fixture.Create<ShortenUrlRequest>();
            request.OriginalUrl = "http://example.com";
            _urlShortenerServiceMock.Setup(service => service.ShortenUrl(request.OriginalUrl)).Returns((ShortenUrlResponse)null);

            // Act
            var result = _controller.ShortenUrl(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("URL already shortened.", conflictResult.Value);
        }

        // Test for unexpected error
        [Fact]
        public void ShortenUrl_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = _fixture.Create<ShortenUrlRequest>();
            _urlShortenerServiceMock.Setup(service => service.ShortenUrl(It.IsAny<string>())).Throws(new Exception("Unexpected error"));

            // Act
            var result = _controller.ShortenUrl(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // Test for URL not found
        [Fact]
        public void ResolveUrl_ReturnsNotFound_WhenUrlIsNotFound()
        {
            // Arrange
            var shortId = "abc123";
            _urlShortenerServiceMock.Setup(service => service.ResolveUrl(shortId)).Returns((string)null);

            // Act
            var result = _controller.ResolveUrl(shortId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Shortened URL not found.", notFoundResult.Value);
        }

        // Test for unexpected error
        [Fact]
        public void ResolveUrl_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var shortId = "abc123";
            _urlShortenerServiceMock.Setup(service => service.ResolveUrl(It.IsAny<string>())).Throws(new Exception("Unexpected error"));

            // Act
            var result = _controller.ResolveUrl(shortId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
