using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileDirectory.Models;
using File = FileDirectory.Models.File;

namespace FileDirectory.DataAccess
{
    public class FileDirectoryService : IFileDirectoryRepository
    {
        private string _baseUri;
        
        public FileDirectoryService(string baseUri)
        {
            _baseUri = baseUri;
        }
        
        public bool DirectoryExists(string upperDirectory, string lowerDirectory)
        {
            return Directory.Exists($"{_baseUri}/{upperDirectory}/{lowerDirectory}");
            
            // What if lowerDirectory null?
            // In future would build path dynamically skipping lowerDirectory if not provided
        }

        public async Task<IEnumerable<File>> FetchFiles(string upperDirectory, string lowerDirectory)
        {
            var filesToReturn = new List<File>();
            
            var htmlFileNames = GetFilesNamesByType(upperDirectory, lowerDirectory, FileType.html);
            var pdfFiles = GetFilesNamesByType(upperDirectory, lowerDirectory, FileType.pdf);
            var xmlFiles = GetFilesNamesByType(upperDirectory, lowerDirectory, FileType.xls);

            filesToReturn.AddRange(await ExtractContent(htmlFileNames));
            filesToReturn.AddRange(await ExtractContent(pdfFiles));
            filesToReturn.AddRange(await ExtractContent(xmlFiles));
            
            return filesToReturn;
        }

        private static IEnumerable<File> TrimOlderVersions(IEnumerable<File> fileNames)
        {
            
            var groupedFilesByVersions = fileNames
                .Select(c =>
                    new
                    {
                        SplitArray = c.FileName.Split('.'),
                        Value = c
                    })
                .Select(c => new
                {
                    FileName = c.SplitArray.First(),
                    Version = c.SplitArray.Last(),
                    Value = c.Value,
                })
                .GroupBy(r => r.FileName);
            

            return groupedFilesByVersions.Select
                (fileType => fileType.OrderByDescending(x => x.Version)
                .Select(x => x.Value).First()).ToList();
        }

        private static async Task<List<File>> ExtractContent(IEnumerable<File> files)
        {
            var filesToReturn = new List<File>();

            foreach (var file in files)
            {
                file.Content = await System.IO.File.ReadAllTextAsync(file.FilePath);
                filesToReturn.Add(file);
            }

            return filesToReturn;
        }

        private IEnumerable<File> GetFilesNamesByType(string upperDirectory, string lowerDirectory, FileType fileType)
        {
            var files = new List<File>();
            
            var filePaths = Directory.GetFiles($"{_baseUri}/{upperDirectory}/{lowerDirectory}",
                $"*.{fileType}");
            
            foreach (var file in filePaths)
            {
                var path = file;
                
                files.Add(
                    new File
                    {
                        FileName = Path.GetFileNameWithoutExtension(file),
                        FilePath = file,
                        Content = null,
                        FileType = fileType
                    });
                
            }
            
            return TrimOlderVersions(files);
            //
            // // file path, name, type
            //
            // // file path, 
            // fileName = Path.GetFileName(filePaths[0]);
            // return Directory.GetFiles($"{_baseUri}/{upperDirectory}/{lowerDirectory}",
            //     $"*.{fileType}");
        }
    }
}