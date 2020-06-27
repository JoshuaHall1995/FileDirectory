using System.Collections.Generic;
using FileDirectory.Models;

namespace FileDirectory.Handlers
{
    public interface IFileHandler
    {
        void Handle(File files);
    }
}