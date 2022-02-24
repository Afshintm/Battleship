using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;
using EnvironmentName = Microsoft.Extensions.Hosting.Environments;

namespace SampleTests.Components.Tests
{
    public static class InMemoryWebServer
    {
        private static readonly Policy CreateTestServerPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(10));

        public static async Task<TestServer> CreateServerAsync(Action<IServiceCollection> configureServices = null, bool useSqs = false)
        {
            var result = await CreateTestServerPolicy.ExecuteAsync(async () =>
            {
                TestServer taskServer = await Task.Run(() =>
                {
                    var configurationBuilder = new ConfigurationBuilder();
                    configurationBuilder.AddJsonFile("appsettings.Development.json");
                    configurationBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>()
                        {
                            {"TracingSettings:EnableTracing", "false"},
                            //{"DomainEventsService:UseQueue", useSqs.ToString().ToLower()},
                            {"DatabaseSettings:InitialiseData", "false"}
                        }
                    );

                    var configuration = configurationBuilder.Build();

                    var loggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Warning()
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                    Log.Logger = loggerConfiguration.CreateLogger();

                    Environment.SetEnvironmentVariable("DomainEventsService__UseQueue", useSqs.ToString().ToLower());

                    var webHostBuilder = WebHost.CreateDefaultBuilder()
                        .UseEnvironment(EnvironmentName.Development)
                        .UseConfiguration(configuration)
                        .UseSerilog()
                        .ConfigureServices(services =>
                            {
                                services.AddSingleton(new ServiceOverride(configureServices));
                                services.AddSingleton(new ConfigurationOverride(configuration));
                            }
                        )
                        .UseStartup<ControlledStartup>();

                    var server = new TestServer(webHostBuilder);

                    DatabaseFixture.Instance.SetupDatabase();

                    return server;
                });

                return taskServer;
            });

            return result;
        }

        public class ServiceOverride
        {
            private readonly Action<IServiceCollection> _configureServices;

            public ServiceOverride(Action<IServiceCollection> configureServices)
            {
                _configureServices = configureServices;
            }

            public void Override(IServiceCollection services)
            {
                InsertInMemorySqsClientFactory(services);
                _configureServices?.Invoke(services);
            }

            private static void InsertInMemorySqsClientFactory(IServiceCollection services)
            {
                var sqsClientFactory = new InMemorySqsClientFactory();
                services.AddTransient<ISqsClientFactory>(_ => sqsClientFactory);
            }
        }

        public class ConfigurationOverride
        {
            private readonly IConfigurationRoot _configurationToAppend;

            public ConfigurationOverride(IConfigurationRoot configurationToAppend)
            {
                _configurationToAppend = configurationToAppend;
            }

            public IConfiguration EnhanceConfiguration(IConfiguration seedConfiguration)
            {
                var configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddConfiguration(seedConfiguration);
                configurationBuilder.AddConfiguration(_configurationToAppend);

                return configurationBuilder.Build();
            }
        }

        public sealed class ControlledStartup : IStartup
        {
            private readonly ServiceOverride _serviceOverride;
            private readonly Startup _applicationStartup;

            public ControlledStartup(
                IConfiguration seedConfiguration,
                IWebHostEnvironment environment,
                ServiceOverride serviceOverride,
                ConfigurationOverride configurationOverride
            )
            {
                _serviceOverride = serviceOverride;

                var enhancedConfiguration = configurationOverride.EnhanceConfiguration(seedConfiguration);
                _applicationStartup = new Startup(enhancedConfiguration, environment);
            }

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                _applicationStartup.ConfigureServices(services);
                _serviceOverride.Override(services);
                return services.BuildServiceProvider();
            }

            public void Configure(IApplicationBuilder app)
            {
                var hostingEnvironment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                _applicationStartup.Configure(app, hostingEnvironment);
            }
        }

        private static TestServer _testServer;

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public static async Task<HttpClient> GetHttpClient()
        {
            if (_testServer == null)
            {
                await Semaphore.WaitAsync();
                if (_testServer == null)
                {
                    try
                    {
                        try
                        {
                            _testServer = await CreateServerAsync();
                        }
                        catch
                        {
                            _testServer = await CreateServerAsync();
                        }

                    }
                    finally
                    {
                        Semaphore.Release();
                    }
                }
                else
                {
                    Semaphore.Release();
                }
            }

            return _testServer.CreateClient();
        }

        public static Func<Task<HttpClient>> GetHttpClientCreator(this TestServer testServer)
        {
            return () => Task.FromResult(testServer.CreateClient());
        }
    }
}
