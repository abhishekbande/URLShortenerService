using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Services.Interfaces;

namespace UrlShortener.Services
{
    /// <summary>
    /// In-memory implementation of the cache service using memory cache.
    /// </summary>
    public class InMemoryCacheService : ICacheService
    {
        // ConcurrentDictionary used as an in-memory cache to store key-value pairs.
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<InMemoryCacheService> _logger;
        private int cacheDuration;

        public InMemoryCacheService(IMemoryCache memoryCache, ILogger<InMemoryCacheService> logger, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            cacheDuration = int.Parse(configuration["CacheDurationInHours"] ?? "24");
        }

        /// <summary>
        /// Sets a value in the in-memory cache with the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to store.</param>
        public void Set(string key, string value)
        {
            try
            {
                _memoryCache.Set(key, value, TimeSpan.FromHours(cacheDuration));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
        }

        /// <summary>
        /// Gets a value from the in-memory cache by the specified key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>The value associated with the key, or null if not found.</returns>
        public string Get(string key)
        {
            try
            {
                _memoryCache.TryGetValue(key, out string value);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
            }
        }
    }
}
