using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using UrlShortener.Models;
using UrlShortener.Services.Interfaces;
using UrlShortner.Repositories;

namespace UrlShortener.Services
{
    /// <summary>
    /// Service for shortening URLs and resolving shortened URLs.
    /// Uses a repository to store URL mappings and a cache service for fast lookups.
    /// </summary>
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UrlShortenerService> _logger;

        public UrlShortenerService(
            IUrlRepository urlRepository,
            ICacheService cacheService, 
            ILogger<UrlShortenerService> logger)
        {
            _urlRepository = urlRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Shortens the provided original URL.
        /// </summary>
        /// <param name="originalUrl">The original URL to be shortened.</param>
        /// <returns>A shortened URL in the format of http://<domain>/<short_id>, or null if it already exists.</returns>
        public ShortenUrlResponse? ShortenUrl(string originalUrl)
        {
            try
            {
                ShortenUrlResponse? shortenUrlResponse = null;

                // Check if URL already exists in the repository or cache
                var shortId = _urlRepository.GetShortIdByOriginalUrl(originalUrl);
                if (shortId == null)
                {
                    // Generate new short ID if the URL is not found
                    shortId = GenerateShortId(originalUrl);

                    // Add the URL mapping to the repository and cache it for future use.
                    _urlRepository.AddUrl(shortId, originalUrl);
                    _cacheService.Set(shortId, originalUrl);
                }

                return new ShortenUrlResponse()
                {
                    ShortUrl = $"http://{AppDomain.CurrentDomain.FriendlyName}/{shortId}",
                    ShortId = shortId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error shortening URL: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resolves a shortened URL back to its original URL.
        /// </summary>
        /// <param name="shortId">The unique identifier for the shortened URL.</param>
        /// <returns>The original URL associated with the short ID, or null if not found.</returns>
        public string ResolveUrl(string shortId)
        {
            try
            {
                var originalUrl = _cacheService.Get(shortId);
                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    // If the URL is not found in the cache, retrieve it from the repository.
                    originalUrl = _urlRepository.GetUrlByShortId(shortId);

                    if (originalUrl != null)
                    {
                        // Cache the result for faster future access.
                        _cacheService.Set(shortId, originalUrl);
                    }
                }

                return originalUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resolving URL: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates a short ID for the given original URL using a simple GUID-based approach.
        /// </summary>
        /// <param name="originalUrl">The original URL to generate a short ID for.</param>
        /// <returns>A unique shortened ID.</returns>
        private static string GenerateShortId(string originalUrl)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Substring(0, 8);
        }
    }
}
