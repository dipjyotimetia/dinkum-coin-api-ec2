using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using App.Metrics;
using App.Metrics.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace DinkumCoin.Api.Mvc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {

            services.AddMvcCore(options => options
                                       .Filters.Add(new ExceptionFilter())
                                       )
                .AddApiExplorer()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddDataAnnotations()
                .AddJsonFormatters(settings =>
                {
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.Converters.Add(new StringEnumConverter());
                });

            services
                .AddMvc()
                .AddMetrics();


            return services;
        }
    }
}
