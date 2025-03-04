using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Repositories
{
    /// <summary>
    /// UrlRepository provides in-memory storage for mapping shortened URLs to their original URLs.
    /// It supports adding new URL mappings and retrieving the original URL given a short ID.
    /// This class uses a ConcurrentDictionary to ensure thread safety when accessing and modifying the mappings.
    /// </summary>
    public interface IUrlRepository
    {
        /// <summary>
        /// Adds a new URL mapping (short URL to original URL) to the repository.
        /// </summary>
        /// <param name="shortId">The unique identifier for the shortened URL.</param>
        /// <param name="originalUrl">The original URL that needs to be shortened.</param>
        void AddUrl(string shortId, string originalUrl);

        /// <summary>
        /// Retrieves the original URL for a given short ID.
        /// </summary>
        /// <param name="shortId">The unique identifier for the shortened URL.</param>
        /// <returns>The original URL associated with the short ID.</returns>
        string GetUrlByShortId(string shortId);

        /// <summary>
        /// Retrieves the short Id for a given Original url.
        /// </summary>
        /// <param name="originalUrl">The orignal URL.</param>
        /// <returns>The short Id associated with the original Url.</returns>
        string GetShortIdByOriginalUrl(string originalUrl);
    }
}
