using System.Collections.Generic;
using FileDirectory.Models;

namespace FileDirectory.Handlers
{
    public class PdfFileHandler : IFileHandler
    {
        public void Handle(File files)
        {
            // Anything needed for: 
            //PDFs should be rendered by the browser where possible. (We’ll use Chrome to test.)            
            throw new System.NotImplementedException();
        }
    }
}