using System.Reflection;

namespace DinkumCoin.Api.Configuration
{
    public static class ApplicationVersion
    {
        public static string Value { get; } =
            typeof(ApplicationVersion)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
    }
}
