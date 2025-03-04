using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Models;

namespace UrlShortener.Services.Interfaces
{
    /// <summary>
    /// Interface defining the contract for a URL shortener service.
    /// This service provides methods to shorten a URL and resolve a shortened URL.
    /// </summary>
    public interface IUrlShortenerService
    {
        /// <summary>
        /// Shortens the provided original URL.
        /// </summary>
        /// <param name="originalUrl">The original URL to be shortened.</param>
        /// <returns>A ShortenUrlResponse, or null if the URL could not be shortened.</returns>
        ShortenUrlResponse? ShortenUrl(string originalUrl);

        /// <summary>
        /// Resolves a shortened URL back to its original URL.
        /// </summary>
        /// <param name="shortId">The unique identifier of the shortened URL.</param>
        /// <returns>The original URL associated with the short ID, or null if not found.</returns>
        string ResolveUrl(string shortId);
    }
}
