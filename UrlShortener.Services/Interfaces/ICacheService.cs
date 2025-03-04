using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Services.Interfaces
{
    /// <summary>
    /// Interface defining the contract for a cache service.
    /// This service provides methods to set and get key-value pairs in a cache.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Sets a value in the cache with the specified key.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to store.</param>
        void Set(string key, string value);

        /// <summary>
        /// Gets a value from the cache by its key.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>The value associated with the key, or null if not found.</returns>
        string Get(string key);
    }
}
