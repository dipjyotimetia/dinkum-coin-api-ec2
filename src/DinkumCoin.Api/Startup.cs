using DinkumCoin.Api.Mvc;
using DinkumCoin.Core.Contracts;
using DinkumCoin.Data.Repositories;
using DinkumCoin.Services;
using Microsoft.AspNetCore.Builder;
using App.Metrics;
using App.Metrics.AspNetCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Hosting;

namespace DinkumCoin.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {
            services
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                })
                .AddOptions()
                .AddMvcServices();
            
            services.AddTransient<IMathService, MathService>();
            services.AddTransient<IMiningService, MiningService>();
            if (env.IsEnvironment("local"))
            {
                services.TryAddSingleton<IDinkumRepository, InMemoryRepository>();
            }
            else
            {
                services.TryAddSingleton<IDinkumRepository, DynamoRepository>();
            }
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DinkumCoin Service", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseCors("CorsPolicy");
            app.UseMvcServices();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DinkumCoin Service V1");
            });
        }
    }
}
