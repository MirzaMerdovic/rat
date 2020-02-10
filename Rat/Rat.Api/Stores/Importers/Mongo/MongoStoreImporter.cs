using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rat.Api.Stores.Importers.Mongo.Collection;
using Rat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo
{
    public class MongoStoreImporter : IStoreImporter
    {
        public string Type => "MongoDb";

        private readonly MongoStoreOptions _options;
        private readonly IMongoCollectionFactory _collectionFactory;

        public MongoStoreImporter(IOptionsMonitor<MongoStoreOptions> options, IMongoCollectionFactory collectionFactory)
        {
            _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
            _collectionFactory = collectionFactory ?? throw new ArgumentNullException(nameof(collectionFactory));
        }

        public async Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            if (!_options.Enabled)
                return Enumerable.Empty<ConfigurationEntry>();

            IMongoCollection<MongoConfigurationEntry> collection = await _collectionFactory.Get<MongoConfigurationEntry>(cancellation).ConfigureAwait(false);

            var cursor = await collection.FindAsync(x => x.Key != null, null, cancellation).ConfigureAwait(false);

            var entries = await cursor.ToListAsync().ConfigureAwait(false);

            return entries;
        }
    }
}