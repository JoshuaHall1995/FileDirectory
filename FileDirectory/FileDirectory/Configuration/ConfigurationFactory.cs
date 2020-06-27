using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileDirectory.Configuration
{
    public static class ConfigurationFactory
    {
        public static T ApplicationConfiguration<T>() where T : new()
        {
            var settings = new T();
            var configRoot = GetConfigurationRoot();
            configRoot.Bind(settings);
            return settings;
        }
        private static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true)
                .Build();
        }
    }
}