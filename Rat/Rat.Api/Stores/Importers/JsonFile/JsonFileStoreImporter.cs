using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Rat.Api.Stores.Importers.JsonFile
{
    public class JsonFileStoreImporter : IStoreImporter
    {
        private readonly JsonFileStoreOptions _options;
        private readonly ILogger _logger;

        public string Type => "JsonFile";

        public JsonFileStoreImporter(IOptionsMonitor<JsonFileStoreOptions> options, ILogger<JsonFileStoreImporter> logger)
        {
            _options = options.CurrentValue ?? new JsonFileStoreOptions();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            if (!_options.Enabled)
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

        // TODO: Use stream and JsonReader to optimize memory when loading big json files.
        private static async Task<IEnumerable<ConfigurationEntry>> Load(string path, ILogger logger)
        {
            try
            {
                var jsonFile = await File.ReadAllTextAsync(path).ConfigureAwait(false);
                var entries = JsonConvert.DeserializeObject<IEnumerable<ConfigurationEntry>>(jsonFile);
                jsonFile = string.Empty;

                return entries;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to deserialize JSON file content. Path: {path}", ex);

                return Enumerable.Empty<ConfigurationEntry>();
            }
        }
    }
}