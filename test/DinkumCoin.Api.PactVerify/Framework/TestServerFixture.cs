using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO;
using DinkumCoin.Api.Mvc;
using DinkumCoin.Core.Contracts;
using DinkumCoin.Services;
using DinkumCoin.Data.Repositories;

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFileAlternate;

using App.Metrics;
using App.Metrics.AspNetCore;
using System;
using System.Net;
using DinkumCoin.Api.Configuration;

namespace DinkumCoin.Api.PactVerify.Framework
{
public class TestServerFixture
    {
        public IWebHost CreateWebHost(string baseUrl)
        {
            return new WebHostBuilder()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseUrls(baseUrl)
               .UseKestrel()
                .ConfigureMetricsWithDefaults(builder =>
                {
                    builder.Report.ToConsole(TimeSpan.FromSeconds(2));
                })
                .UseStartup<Startup>()
                .UseMetrics()
               .UseDefaultServiceProvider(options => options.ValidateScopes = true)
               .ConfigureAppConfiguration(
                   (hostingContext, config) => config
                       .AddEnvironmentVariables("DinkumCoin_")
                        .AddJsonFile($"appsettings.json", false))
                .ConfigureLogging((hostingContext, logging) =>
                    logging.AddProvider(CreateLoggerProvider(hostingContext.Configuration)))
                .Build();
               
        }

        /*
         * 
         * 
         *         {
            new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://*:5000")
                .UseKestrel()
                .ConfigureMetricsWithDefaults(builder =>
                {
                    builder.Report.ToConsole(TimeSpan.FromSeconds(2));
                })
                
                .UseStartup<Startup>()
                .UseMetrics()
                .UseDefaultServiceProvider(options => options.ValidateScopes = true)
                .ConfigureAppConfiguration(
                    (hostingContext, config) => config
                        .AddEnvironmentVariables("DinkumCoin_")
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true)
                        .AddCommandLine(args))
                .ConfigureLogging((hostingContext, logging) =>
                    logging.AddProvider(CreateLoggerProvider(hostingContext.Configuration)))
                .Build()
                .Run();
        }*/







        private static SerilogLoggerProvider CreateLoggerProvider(IConfiguration configuration)
        {
            LoggerConfiguration logConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFileAlternate(new JsonFormatter(), "./logs", fileSizeLimitBytes: 10000000, retainedFileCountLimit: 30)
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ApplicationVersion", ApplicationVersion.Value)
                .Enrich.WithProperty("Hostname", Dns.GetHostName())
                .Enrich.FromLogContext();

            return new SerilogLoggerProvider(logConfig.CreateLogger());
        }

    }
}