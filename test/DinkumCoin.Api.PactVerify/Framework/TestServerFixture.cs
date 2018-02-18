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
                .Configure(app => app
                 //  .UseMiddleware<ProviderStateMiddleware>()
                   .UseMvc()
                   )
                .ConfigureServices( services => {services
                    .AddOptions()
                    .AddMvcServices();
                    services.AddTransient<IMathService, MathService>();
                    services.AddTransient<IMiningService, MiningService>();
                    services.TryAddSingleton<IDinkumRepository, InMemoryRepository>();
                }) 
               .UseDefaultServiceProvider(options => options.ValidateScopes = true)
               .ConfigureAppConfiguration(
                   (hostingContext, config) => config
                       .AddEnvironmentVariables("DinkumCoin_")
                    .AddJsonFile($"appsettings.json", false))
                .Build();
               
        }
    }
}