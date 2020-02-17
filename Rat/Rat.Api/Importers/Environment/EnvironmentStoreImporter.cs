using Newtonsoft.Json;
using Rat.Api.Importers;
using Rat.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers
{
    public class EnvironmentStoreImporter : IStoreImporter
    {
        public int Rank => int.MaxValue;

        public string Type => "Environment";

        public Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            IDictionary variables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);

            if (variables.Count == 0)
                return Task.FromResult(Enumerable.Empty<ConfigurationEntry>());

            var entries = new List<ConfigurationEntry>();

            foreach (var key in variables.Keys)
            {
                if (!key.ToString().StartsWith("rat", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var value = variables[key];
                var jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(value.ToString()));
                var entry = JsonConvert.DeserializeObject<ConfigurationEntry>(jsonPayload);

                entries.Add(entry);
            }

            return Task.FromResult<IEnumerable<ConfigurationEntry>>(entries);
        }
    }
}