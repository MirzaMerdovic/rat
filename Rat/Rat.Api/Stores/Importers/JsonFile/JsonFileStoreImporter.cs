using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rat.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.JsonFile
{
    public class JsonFileStoreImporter : IStoreImporter
    {
        private static readonly Func<JsonFileStoreOptions, int> GetRank = delegate (JsonFileStoreOptions options) { return options.Rank; };

        private readonly JsonFileStoreOptions _options;
        private readonly ILogger _logger;

        public int Rank => GetRank(_options);

        public string Type => "JsonFile";

        public JsonFileStoreImporter(IOptionsMonitor<JsonFileStoreOptions> options, ILogger<JsonFileStoreImporter> logger)
        {
            _options = options.CurrentValue ?? new JsonFileStoreOptions();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            if (Rank == 0)
            {
                _logger.LogInformation("JsonFile Store is disabled.");

                return Enumerable.Empty<ConfigurationEntry>();
            }

            if (!File.Exists(_options.Path))
            {
                _logger.LogInformation($"JSON File: {_options.Path} doesn not exist.");

                return Enumerable.Empty<ConfigurationEntry>();
            }

            if (!Path.GetExtension(_options.Path).Equals(".json", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation($"JSON file with invalid extension: {Path.GetExtension(_options.Path)}. Expected: .json");

                return Enumerable.Empty<ConfigurationEntry>();
            }

            var entries = await Load(_options.Path, _logger).ConfigureAwait(false);
            _logger.LogInformation($"Loaded {entries.Count()} entries from JSON file");

            return entries;
        }

        private static async Task<IEnumerable<ConfigurationEntry>> Load(string path, ILogger logger)
        {
            var serializer = new JsonSerializer();
            var entries = new List<ConfigurationEntry>(0);
            Stream stream = null;

            try
            {
                stream = File.OpenRead(path);
                using (var reader = new JsonTextReader(new StreamReader(stream)))
                {
                    stream = null;

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        if (reader.TokenType != JsonToken.StartObject)
                            continue;

                        var entry = serializer.Deserialize<ConfigurationEntry>(reader);
                        entries.Add(entry);
                    }
                }

                return entries;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to deserialize JSON file content. Path: {path}", ex);

                return Enumerable.Empty<ConfigurationEntry>();
            }
            finally
            {
                stream?.Dispose();
            }
        }
    }
}