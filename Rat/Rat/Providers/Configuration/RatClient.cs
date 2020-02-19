using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    public sealed class RatClient : IRatClient
    {
        private static readonly Func<HttpRequestException, bool> CheckError = delegate (HttpRequestException x)
        {
            if (!x.Data.Contains(nameof(HttpStatusCode)))
                return false;

            var statusCode = (HttpStatusCode)x.Data[nameof(HttpStatusCode)];

            if (statusCode < HttpStatusCode.InternalServerError)
                return statusCode == HttpStatusCode.RequestTimeout;

            return false;
        };

        private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = delegate (HttpResponseMessage response)
        {
            if (response.StatusCode < HttpStatusCode.InternalServerError)
                return response.StatusCode == HttpStatusCode.RequestTimeout;

            return true;
        };

        private readonly IList<string> _keys = new List<string>();

        private readonly RatClientOptions _options;

        private readonly HttpClient _client;

        private readonly IMemoryCache _cache;

        private readonly IRetryProvider _retry;

        public RatClient(IOptionsMonitor<RatClientOptions> options, IMemoryCache cache, IRetryProvider retry, IHttpClientFactory factory)
        {
            _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _retry = retry ?? throw new ArgumentNullException(nameof(retry));
            _client = Initialize(factory, _options.ApiBaseUrl);

            static HttpClient Initialize(IHttpClientFactory factory, string baseUrl)
            {
                var client = factory.CreateClient();

                if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
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
                        () => _client.GetAsync(new Uri($"api/Configuration/{key}", UriKind.Relative), cancellation))
                    .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new ConfigurationEntryRetrievalFailed(key, response.ReasonPhrase);

            var payload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var entity = JsonConvert.DeserializeObject<ConfigurationEntry>(payload);

            _ = entity ?? throw new ConfigurationEntryNotFoundException(key);

            return entity;
        }

        private void SaveConfigurationEntity(ConfigurationEntry entity)
        {
            _keys.Add(entity.Key);
            var expiration = TimeSpan.FromSeconds(_options.CacheExpiry);
            _cache.Set(entity.Key, entity.Value, expiration);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _cache?.Dispose();
        }
    }
}