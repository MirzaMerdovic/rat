using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rat.Api.DataAccess.Mongo.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.DataAccess.Mongo.Collection
{
    public abstract class MongoCollectionFactory<TOptions> : IMongoCollectionFactory where TOptions : MongoCollectionOptions, new()
    {
        private readonly IMongoDatabaseFactory _databaseFactory;
        private readonly TOptions _options;

        public MongoCollectionFactory(IOptions<TOptions> options, IMongoDatabaseFactory databaseFactory)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
        }

        public async Task<IMongoCollection<T>> Get<T>(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.Name))
                throw new ArgumentNullException("CollectionName not configured.");

            var database = await _databaseFactory.Get(cancellationToken).ConfigureAwait(false);
            IMongoCollection<T> collection = database.GetCollection<T>(_options.Name);

            return collection;
        }
    }
}