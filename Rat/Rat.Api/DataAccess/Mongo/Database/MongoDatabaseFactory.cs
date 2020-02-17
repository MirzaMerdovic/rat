using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rat.Api.DataAccess.Mongo.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.DataAccess.Mongo.Database
{
    public sealed class MongoDatabaseFactory : IMongoDatabaseFactory
    {
        private readonly ConcurrentDictionary<string, IMongoDatabase> _cache = new ConcurrentDictionary<string, IMongoDatabase>();

        private readonly MongoDatabaseOptions _options;
        private readonly IMongoClient _client;
        private readonly ILogger _logger;

        public MongoDatabaseFactory(IOptions<MongoDatabaseOptions> options, IMongoClientFactory clientFactory, ILogger<MongoDatabaseFactory> logger)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _ = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _client = clientFactory.Create();
        }

        public async Task<IMongoDatabase> Get(CancellationToken cancellationToken)
        {
            if (_cache.ContainsKey(_options.Name))
                return _cache[_options.Name];

            var names = await (await _client.ListDatabaseNamesAsync(cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);

            if (!names.Any(x => x.Equals(_options.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentOutOfRangeException($"Database: {_options.Name} doesn't exist.");

            _cache[_options.Name] = _client.GetDatabase(_options.Name);
            _logger.LogInformation($"Database initialized {_options.Name}.");

            return _cache[_options.Name];
        }
    }
}