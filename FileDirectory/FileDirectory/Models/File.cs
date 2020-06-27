using System.Runtime.CompilerServices;

namespace FileDirectory.Models
{
    public class File
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
        public string Content { get; set; }
        public FileType FileType { get; set; }
    }
}