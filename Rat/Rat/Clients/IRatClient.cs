using Rat.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Clients
{
    public interface IRatClient : IDisposable
    {
        /// <summary>
        /// Removes the key from the cache.
        /// </summary>
        /// <param name="key">Cache key</param>
        void InvalidateKey(string key);

        /// <summary>
        /// Clears the cache contents.
        /// </summary>
        void ClearCache();

        Task RegisterClient(ClientRegistrationEntry entry, CancellationToken cancellation);

        /// <summary>
        /// Loads all the keys that have been specified in the configuration.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task LoadAll(CancellationToken cancellation);

        /// <summary>
        /// Retrieves the cached value.
        /// </summary>
        /// <typeparam name="T">The type of retrieved entry.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns></returns>
        Task<T> GetValue<T>(string key, CancellationToken cancellation);
    }
}