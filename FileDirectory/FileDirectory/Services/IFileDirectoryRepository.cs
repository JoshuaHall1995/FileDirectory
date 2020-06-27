using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileDirectory.Models;

namespace FileDirectory.DataAccess
{
    public interface IFileDirectoryRepository
    {
        public bool DirectoryExists(string upperDirectory, string lowerDirectory);
        public Task<IEnumerable<File>> FetchFiles(string upperDirectory, string lowerDirectory);

    }
}