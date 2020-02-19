using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Rat.Api.DataAccess.Mongo.Collection;
using Rat.Data;
using Rat.Providers.Resiliency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores
{
    public interface IClientStore
    {
        Task<string> Register(ClientRegistrationEntry entry, CancellationToken cancellation);

        Task Notify(IEnumerable<ConfigurationEntry> keys, CancellationToken cancellation);
    }

    public class ClientStore : IClientStore
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

        private readonly IMongoCollectionFactory _collectionFactory;
        private readonly IHttpClientFactory _factory;
        private readonly IRetryProvider _retryProvider;
        private readonly ILogger _logger;

        public ClientStore(IMongoCollectionFactory collectionFactory, IHttpClientFactory factory, IRetryProvider retryProvider, ILogger<ClientStore> logger)
        {
            _collectionFactory = collectionFactory ?? throw new ArgumentNullException(nameof(collectionFactory));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _retryProvider = retryProvider ?? throw new ArgumentNullException(nameof(retryProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Notify(IEnumerable<ConfigurationEntry> updated, CancellationToken cancellation)
        {
            var collection = await _collectionFactory.Get<ClientRegistrationEntry>(cancellation).ConfigureAwait(false);
            var clients = await collection.FindAsync(_ => true, null, cancellation).ConfigureAwait(false);
            var entries = await clients.ToListAsync().ConfigureAwait(false);

            if (!entries.Any())
                return;

            var client = _factory.CreateClient("notifier");
            using var content = new StringContent(JsonConvert.SerializeObject(updated.Select(x => new { Key = x.Key, Value = x.Value })));

            foreach (var entry in entries)
            {
                var response =
                    await
                        _retryProvider.RetryOn<HttpRequestException, HttpResponseMessage>(
                            CheckError,
                            TransientHttpStatusCodePredicate,
                            () => client.PostAsync(new Uri(entry.Endpoint), content, cancellation))
                        .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    continue;

                _logger.LogWarning($"Failed to notify client: {entry.Endpoint} received status: {response.StatusCode} with reason: {response.ReasonPhrase ?? "None provided"}");
            }
        }

        public async Task<string> Register(ClientRegistrationEntry entry, CancellationToken cancellation)
        {
            _ = entry ?? throw new ArgumentNullException(nameof(entry));

            var collection = await _collectionFactory.Get<ClientRegistrationEntry>(cancellation).ConfigureAwait(false);

            var clients = await collection.FindAsync(x => x.Endpoint == entry.Endpoint, null, cancellation).ConfigureAwait(false);
            var client = await clients.FirstOrDefaultAsync(cancellation).ConfigureAwait(false);

            if (client == null)
                await collection.InsertOneAsync(entry, null, cancellation).ConfigureAwait(false);

            return entry.Id.ToString();
        }
    }
}