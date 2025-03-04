using FluentValidation;
using FluentValidation.Results;
using Xunit;
using UrlShortener.Models;
using UrlShortner.Api.Validators;

namespace UrlShortener.Tests.Validators
{
    public class UrlRequestValidatorTests
    {
        private readonly UrlRequestValidator _validator;

        public UrlRequestValidatorTests()
        {
            _validator = new UrlRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OriginalUrl_Is_Empty()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = string.Empty };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OriginalUrl" && e.ErrorMessage == "Original URL cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_OriginalUrl_Is_Invalid()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = "invalid_url" };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OriginalUrl" && e.ErrorMessage == "Invalid URL format.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_OriginalUrl_Is_Valid()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = "https://www.example.com" };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_OriginalUrl_Has_Invalid_Scheme()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = "ftp://example.com" };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OriginalUrl" && e.ErrorMessage == "Invalid URL format.");
        }

        [Fact]
        public void Should_Have_Error_When_OriginalUrl_Is_Only_Scheme()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = "https://" };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OriginalUrl" && e.ErrorMessage == "Invalid URL format.");
        }

        [Fact]
        public void Should_Have_Error_When_OriginalUrl_Has_Missing_TLD()
        {
            // Arrange
            var model = new ShortenUrlRequest { OriginalUrl = "https://example" };

            // Act
            ValidationResult result = _validator.Validate(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OriginalUrl" && e.ErrorMessage == "Invalid URL format.");
        }
    }
}
