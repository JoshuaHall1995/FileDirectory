using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileDirectory.DataAccess;
using FileDirectory.Handlers;
using FileDirectory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace FileDirectory
{
    public class GetDirectoryContentFunction
    {
        [FunctionName("getDirectoryContent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,
                "GET",
                Route = "/{upperDirectory}/getcontent")]
            HttpRequestMessage request,
            string upperDirectory)
        {
            var repository = new FileDirectoryService(FileDirectorySettings.Current.BaseDirectoryUrl);
            var innerDirectory = GetInnerDirectory(request);

            if (!repository.DirectoryExists(upperDirectory, innerDirectory)) 
                return new BadRequestObjectResult("Supplied directory does not exist");
            
            var files = await repository.FetchFiles(upperDirectory, innerDirectory);

            foreach (var file in files)
            {
                switch (file.FileType)
                {
                    case FileType.html:
                        new HtmlFileHandler().Handle(file);
                        // do something if i could website
                        break;
                    case FileType.pdf:
                        new PdfFileHandler().Handle(file);
                        // do something if i could website
                        break;
                    case FileType.xls:
                        new ExcelFileHandler().Handle(file);
                        // do something if i could website
                        break;
                    case FileType.Unsupported:
                        // build a error message to throw after the foreach loop
                        break;
                }
            }

            return new OkResult();
            // this would catch custom exceptions throw from the handler level. Need to build up
            // errors for each file that fails. 
        }

        private static string GetInnerDirectory(HttpRequestMessage request)
        {
            return request.Headers.Contains("content")
                ? request.Headers.GetValues("content").First()
                : null;
        }

    }
}