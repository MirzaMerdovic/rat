using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Rat.Api.DataAccess.Mongo;
using Rat.Api.DataAccess.Mongo.Client;
using Rat.Api.DataAccess.Mongo.Collection;
using Rat.Api.DataAccess.Mongo.Database;
using Rat.Api.DataAccess.SqlServer;
using Rat.Api.DataAccess.SqlServer.Connection;
using Rat.Api.Importers;
using Rat.Api.Importers.JsonFile;
using Rat.Api.Importers.Mongo;
using Rat.Api.Importers.SqlServer;
using Rat.Api.Observability;
using Rat.Api.Stores;
using Rat.Api.Stores.Importers;
using Rat.Providers.Resiliency;
using System;

namespace Rat.Api
{
    /// <summary>
    /// OWIN configuration and setup.
    /// </summary>
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes new instance of <see cref="Startup"/>
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .SetBasePath(_env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            services.Configure<JsonFileStoreOptions>(configuration.GetSection(nameof(JsonFileStoreOptions)));
            services.Configure<MongoStoreOptions>(configuration.GetSection(nameof(MongoStoreOptions)));
            services.Configure<MongoConnectionOptions>(configuration.GetSection($"{nameof(MongoStoreOptions)}:{nameof(MongoConnectionOptions)}"));
            services.Configure<MongoDatabaseOptions>(configuration.GetSection($"{nameof(MongoStoreOptions)}:{nameof(MongoDatabaseOptions)}"));
            services.Configure<ConfigurationMongoCollectionOptions>(configuration.GetSection($"{nameof(MongoStoreOptions)}:{nameof(MongoCollectionOptions)}:Configurations"));
            services.Configure<ClientMongoCollectionOptions>(configuration.GetSection($"{nameof(MongoStoreOptions)}:{nameof(MongoCollectionOptions)}:Clients"));
            services.Configure<SqlServerStoreOptions>(configuration.GetSection(nameof(SqlServerStoreOptions)));
            services.Configure<SqlConnectionFactoryOptions>(configuration.GetSection($"{nameof(SqlServerStoreOptions)}:{nameof(SqlConnectionFactoryOptions)}"));

            services.AddLogging(x => x.AddConsole());

            services.AddHealthChecks().AddCheck<BasicHealthCheck>("basic", tags: new[] { "ready", "live" });

            services.AddHttpClient();

            services.AddTransient<IRetryProvider, RetryProvider>();

            services.AddSingleton<IMongoClientFactory, MongoClientFactory>();
            services.AddSingleton<IMongoDatabaseFactory, MongoDatabaseFactory>();
            services.AddSingleton<IConfigurationMongoCollectionFactory, ConfigurationMongoCollectionFactory>();
            services.AddSingleton<IClientMongoCollectionFactory, ClientMongoCollectionFactory>();

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            // Register your types
            services.AddTransient<IStoreImporter, JsonFileStoreImporter>();
            services.AddTransient<IStoreImporter, MongoStoreImporter>();
            //services.AddTransient<IStoreImporter, SqlServerStoreImporter>();
            services.AddTransient<IStoreImporter, EnvironmentStoreImporter>();

            services.AddSingleton<IConfigurationStore, ConfigurationStore>();
            services.AddSingleton<IClientStore, ClientStore>();

            // Refer to this article if you require more information on CORS
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors
            void build(CorsPolicyBuilder b) { b.WithOrigins("*").WithMethods("*").WithHeaders("*").Build(); };
            services.AddCors(options => { options.AddPolicy("AllowAllPolicy", build); });

            services
                .AddMvc(
                    options =>
                    {
                        // Refer to this article for more details on how to properly set the caching for your needs
                        // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response
                        options.CacheProfiles.Add(
                            "default",
                            new CacheProfile
                            {
                                Duration = 600,
                                Location = ResponseCacheLocation.None
                            });
                    })
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });

            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 2048;
                options.UseCaseSensitivePaths = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                    async context =>
                    {

                        var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                        loggerFactory.CreateLogger("ExceptionHandler").LogError(exceptionHandler.Error, exceptionHandler.Error.Message, null);
                    });
            });

            app.UseCors("AllowAllPolicy");

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };

                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new[] { "Accept-Encoding" };

                await next().ConfigureAwait(false);
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    AllowCachingResponses = false,
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = HealthReportWriter.WriteResponse
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    AllowCachingResponses = false,
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = HealthReportWriter.WriteResponse
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
