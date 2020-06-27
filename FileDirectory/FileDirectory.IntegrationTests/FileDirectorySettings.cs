using FileDirectory.Configuration;

namespace FileDirectory.IntegrationTests
{
    public class IntegrationTestSettings
    {
        public static IntegrationTestSettings Current { get; } = ConfigurationFactory.ApplicationConfiguration<IntegrationTestSettings>();

        public string BaseDirectoryUrl { get; set; }
    }
}