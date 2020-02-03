using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Rat.Data;
using Rat.Exceptions;
using Rat.Providers.Resiliency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Providers.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = delegate (HttpResponseMessage response)
        {
            if (response.StatusCode < HttpStatusCode.InternalServerError)
                return response.StatusCode == HttpStatusCode.RequestTimeout;

            return true;
        };

        private readonly IList<string> _keys = new List<string>();

        private readonly ConfigurationProviderOptions _options;

        private readonly HttpClient _client;

        private readonly IMemoryCache _cache;

        private readonly IRetryProvider _retry;

        public ConfigurationProvider(IOptionsMonitor<ConfigurationProviderOptions> options, IMemoryCache cache, IRetryProvider retry, IHttpClientFactory factory)
        {
            _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _retry = retry ?? throw new ArgumentNullException(nameof(retry));
            _client = Initialize(factory, _options.ConfigurationApiBaseUrl);

            static HttpClient Initialize(IHttpClientFactory factory, string baseUrl)
            {
                var client = factory.CreateClient();

                if (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
                    throw new ArgumentNullException("ConfigurationApiBaseUrl is missing.");

                client.BaseAddress = new Uri(baseUrl);

                return client;
            }
        }

        public void ClearCache()
        {
            foreach (var key in _keys)
            {
                _cache.Remove(key);
            }

            _keys.Clear();
        }

        public void InvalidateKey(string key)
        {
            if (!_cache.TryGetValue(key, out var _))
                return;

            _cache.Remove(key);
            _keys.Remove(key);
        }

        public async Task<T> GetValue<T>(string key, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue<T>(key, out var item))
                return item;

            var entity = await GetEntity(key, cancellation).ConfigureAwait(false);
            SaveConfigurationEntity(entity);

            return (T)entity.Value;
        }

        public async Task LoadAll(CancellationToken cancellation)
        {
            foreach (var key in _options.KeysForPreLoad)
            {
                if (_cache.TryGetValue(key, out var _))
                    continue;

                var entity = await GetEntity(key, cancellation).ConfigureAwait(false);
                SaveConfigurationEntity(entity);
            }
        }

        private async Task<ConfigurationEntry> GetEntity(string key, CancellationToken cancellation)
        {
            var response =
                await
                    _retry.RetryOn<HttpRequestException, HttpResponseMessage>(
                        CheckError,
                        TransientHttpStatusCodePredicate,
                        () => _client.GetAsync(new Uri(key, UriKind.Relative), cancellation))
                    .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new ConfigurationEntryRetrievalFailed(key, response.ReasonPhrase);

            var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var entity = ConvertBytes<ConfigurationEntry>(bytes);

            if (entity == null)
                throw new ConfigurationEntryNotFoundException(key);

            return entity;

            static T ConvertBytes<T>(byte[] b)
            {
                if (b == null)
                    return default;

                var formatter = new BinaryFormatter();

                using (var stream = new MemoryStream(b))
                {
                    object value = formatter.Deserialize(stream);

                    return (T)value;
                }
            }
        }

        private void SaveConfigurationEntity(ConfigurationEntry entity)
        {
            _keys.Add(entity.Key);
            _cache.Set(entity.Key, entity.Value, entity.Expiration ?? _options.CacheExpiry);
        }

        private bool CheckError(HttpRequestException x)
        {
            if (!x.Data.Contains(nameof(HttpStatusCode)))
                return false;

            var statusCode = (HttpStatusCode)x.Data[nameof(HttpStatusCode)];

            if (statusCode < HttpStatusCode.InternalServerError)
                return statusCode == HttpStatusCode.RequestTimeout;

            return false;
        }

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
                _client.Dispose();

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}