﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;

namespace Rat.Api.DataAccess.SqlServer.Connection
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }

    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private static readonly Func<string, SqlConnection> Create = delegate (string connectionString)
        {
            return new SqlConnection(connectionString);
        };

        private readonly SqlConnectionFactoryOptions _options;

        public SqlConnectionFactory(IOptionsMonitor<SqlConnectionFactoryOptions> options)
        {
            _options = options?.CurrentValue ?? throw new ArgumentNullException(nameof(options));
        }

        public SqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
                throw new ArgumentException("Connection string must be provided.");

            return Create(_options.ConnectionString);
        }
    }
}