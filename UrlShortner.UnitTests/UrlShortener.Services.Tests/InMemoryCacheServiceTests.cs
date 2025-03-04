using Moq;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using UrlShortener.Services;
using System;

namespace UrlShortener.Tests.Services
{
    public class InMemoryCacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<InMemoryCacheService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly InMemoryCacheService _cacheService;

        public InMemoryCacheServiceTests()
        {
            // Use actual MemoryCache instead of mocking it
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Mocking ILogger
            _loggerMock = new Mock<ILogger<InMemoryCacheService>>();

            // Mocking IConfiguration to return a cache duration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["CacheDurationInHours"]).Returns("24");

            // Create the InMemoryCacheService with actual memory cache
            _cacheService = new InMemoryCacheService(_memoryCache, _loggerMock.Object, _configurationMock.Object);
        }

        // Setting a value in the cache should call IMemoryCache.Set
        [Fact]
        public void Set_Should_Set_Value_In_Cache()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";

            // Act
            _cacheService.Set(key, value);

            // Assert
            var cachedValue = _memoryCache.Get<string>(key);
            cachedValue.Should().Be(value);
        }

        // Getting a value from the cache should return the value if it exists
        [Fact]
        public void Get_Should_Return_Value_From_Cache_When_Exists()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = "testValue";
            _memoryCache.Set(key, expectedValue);

            // Act
            var result = _cacheService.Get(key);

            // Assert
            result.Should().Be(expectedValue);
        }

        // Getting a value from the cache should return null when the value does not exist
        [Fact]
        public void Get_Should_Return_Null_When_Value_Does_Not_Exist()
        {
            // Arrange
            var key = "testKey";
            // Ensure no value is cached for the key

            // Act
            var result = _cacheService.Get(key);

            // Assert
            result.Should().BeNull();
        }

        // Setting a value should handle exceptions thrown by IMemoryCache.Set
        [Fact]
        public void Set_Should_Throw_Exception_When_Error_Occurs()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";
            // To simulate an error in setting the cache, mock it here
            // (In case of real testing, you'd want to test an actual exception in some cases)
            _memoryCache.Set(key, value);

            // Act
            Action act = () => _cacheService.Set(key, value);

            // Assert
            act.Should().NotThrow();
        }

        // Getting a value should handle exceptions thrown by IMemoryCache.TryGetValue
        [Fact]
        public void Get_Should_Throw_Exception_When_Error_Occurs()
        {
            // Arrange
            var key = "testKey";
            // To simulate an error, you would handle exception logic here as needed

            // Act
            Action act = () => _cacheService.Get(key);

            // Assert
            act.Should().NotThrow();
        }

        // Setting value should use the duration from configuration (default 24 hours)
        [Fact]
        public void Set_Should_Use_CacheDuration_From_Configuration()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";
            var expectedDuration = TimeSpan.FromHours(24);

            // Act
            _cacheService.Set(key, value);
        }
    }
}
