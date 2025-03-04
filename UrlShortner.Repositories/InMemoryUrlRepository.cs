using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Serialization;

namespace UrlShortner.Repositories
{
    /// <summary>
    /// UrlRepository provides in-memory storage for mapping shortened URLs to their original URLs.
    /// It supports adding new URL mappings and retrieving the original URL given a short ID.
    /// This class uses a ConcurrentDictionary to ensure thread safety when accessing and modifying the mappings.
    /// </summary>
    public class InMemoryUrlRepository : IUrlRepository
    {
        private readonly ConcurrentDictionary<string, string> _urlMappingsByShortId;
        private readonly ConcurrentDictionary<string, string> _urlMappingsByOriginalUrl;
        private readonly ILogger<InMemoryUrlRepository> _logger;

        public InMemoryUrlRepository(ILogger<InMemoryUrlRepository> logger)
        {
            _urlMappingsByShortId = new();
            _urlMappingsByOriginalUrl = new();
            _logger = logger;
        }

        public void AddUrl(string shortId, string originalUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(shortId))
                {
                    throw new ArgumentException("Short ID cannot be null or empty.", nameof(shortId));
                }

                if (string.IsNullOrEmpty(originalUrl))
                {
                    throw new ArgumentException("Original URL cannot be null or empty.", nameof(originalUrl));
                }

                // Handle potential duplicate short ID, if needed
                if (_urlMappingsByShortId.ContainsKey(shortId))
                {
                    throw new InvalidOperationException($"Short ID '{shortId}' already exists.");
                }

                // Check if the original URL already exists in the dictionary
                if (_urlMappingsByOriginalUrl.ContainsKey(originalUrl))
                {
                    throw new InvalidOperationException($"The original URL '{originalUrl}' is already shortened.");
                }

                _urlMappingsByShortId[shortId] = originalUrl;
                _urlMappingsByOriginalUrl[originalUrl] = shortId;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Argument error: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Operation error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public string GetUrlByShortId(string shortId)
        {
            try
            {
                if (string.IsNullOrEmpty(shortId))
                {
                    throw new ArgumentException("Short ID cannot be null or empty.", nameof(shortId));
                }

                // Try to get the original URL
                _urlMappingsByShortId.TryGetValue(shortId, out var originalUrl);

                return originalUrl;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Argument error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public string GetShortIdByOriginalUrl(string originalUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(originalUrl))
                {
                    throw new ArgumentException("Original Url cannot be null or empty.", nameof(originalUrl));
                }

                // Try to get the original URL
                _urlMappingsByOriginalUrl.TryGetValue(originalUrl, out var shortUrl);

                return shortUrl;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Argument error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error: {ex.Message}");
                throw;
            }
        }
    }
}
