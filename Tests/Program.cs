//   "Serilog": {
//     "MinimumLevel": {
//       "Default": "Information",
//       "Override": {
//         "Microsoft": "Warning",
//         "System": "Warning"
//       }
//     },
//     "WriteTo": [
//       {
//         "Name": "Console"
//       }
//     ]
//   },


using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;

namespace sampleTests
{
    public class Program
    {
        private const int PrometheusMetricsPort = 9102;
        private static string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task Main(string[] args)
        {
            Log.Logger = CreateLogger();
            try
            {
                StartMonitoringServer();
                var host = CreateHostBuilder(args).Build();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start up failed.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void StartMonitoringServer()
        {
            var metricServer = new KestrelMetricServer(port: PrometheusMetricsPort);
            metricServer.Start();
        }

        private static ILogger CreateLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Debug();

            if (Env.Equals("development", StringComparison.OrdinalIgnoreCase))
                loggerConfiguration.WriteTo.File("log.ndjson");

            return loggerConfiguration.CreateLogger();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            Log.Information($"Building host with args: {string.Join(",", args)} ,Environment: {Env}");
            var hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.UseSerilog();
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
            hostBuilder.ConfigureAppConfiguration(
                (context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.shared.json", optional: false, reloadOnChange: true);
                }
            );
            return hostBuilder;
        }
    }
}
