﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;

namespace Rat.Api.DataAccess.Mongo.Client
{
    public sealed class MongoClientFactory : IMongoClientFactory
    {
        private readonly ConcurrentDictionary<string, IMongoClient> _cache = new ConcurrentDictionary<string, IMongoClient>();

        private readonly MongoConnectionOptions _options;
        private readonly ILogger _logger;

        public MongoClientFactory(IOptions<MongoConnectionOptions> options, ILogger<MongoClientFactory> logger)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public IMongoClient Create()
        {
            if (_cache.ContainsKey(_options.Name))
                return _cache[_options.Name];

            _cache[_options.Name] = new MongoClient(new MongoUrl(_options.Url));
            _logger.LogInformation($"Client: {_options.Url} created.");

            return _cache[_options.Name];
        }
    }
}