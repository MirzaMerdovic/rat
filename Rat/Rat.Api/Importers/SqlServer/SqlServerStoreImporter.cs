using Microsoft.Extensions.Options;
using Rat.Api.DataAccess.SqlServer.Connection;
using Rat.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Importers.SqlServer
{
    public class SqlServerStoreImporter : IStoreImporter
    {
        private static readonly Func<SqlServerStoreOptions, int> GetRank = delegate (SqlServerStoreOptions options) { return options.Rank; };

        private static readonly Func<string, string> BuildSelectConfigurationQuery = delegate (string database)
        {
            return $"SELECT [Key], [Value], [Expires] FROM [dbo].[{database}] WITH(NOLOCK)";
        };

        public int Rank => GetRank(_options);
        public string Type => "SqlServer";

        private readonly SqlServerStoreOptions _options;
        private readonly ISqlConnectionFactory _connectionFactory;

        public SqlServerStoreImporter(IOptionsMonitor<SqlServerStoreOptions> options, ISqlConnectionFactory connectionFactory)
        {
            _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            if (Rank == 0)
                return Enumerable.Empty<ConfigurationEntry>();

            var entries = new List<ConfigurationEntry>();
            using var connections = _connectionFactory.CreateConnection();

            await connections.OpenAsync(cancellation).ConfigureAwait(false);

            var command = connections.CreateCommand();
            command.CommandText = BuildSelectConfigurationQuery(_options.Database);
            command.CommandType = CommandType.Text;

            var reader = await command.ExecuteReaderAsync(cancellation).ConfigureAwait(false);
            while(await reader.ReadAsync(cancellation).ConfigureAwait(false))
            {
                var key = await reader.GetFieldValueAsync<string>("Key", cancellation).ConfigureAwait(false);
                var value = await reader.GetFieldValueAsync<string>("Value", cancellation).ConfigureAwait(false);

                entries.Add(new ConfigurationEntry { Key = key, Value = value });

            }

            return entries;
        }
    }
}