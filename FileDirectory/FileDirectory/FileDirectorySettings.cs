using FileDirectory.Configuration;

namespace FileDirectory
{
    public class FileDirectorySettings
    {
        public static FileDirectorySettings Current { get; } = ConfigurationFactory.ApplicationConfiguration<FileDirectorySettings>();

        public string BaseDirectoryUrl { get; set; }
    }
}