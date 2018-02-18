using Microsoft.AspNetCore.Builder;

namespace DinkumCoin.Api.Mvc
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMvcServices(this IApplicationBuilder builder)
        {
            return builder
                //  .UseMiddleware<MetricsMiddleware>()
                .UseMvc();
        }
    }
}
