using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileDirectory.DataAccess;
using Xunit;

namespace FileDirectory.IntegrationTests
{
    public class FileDirectoryServiceTests
    {
        private readonly FileDirectoryService _service;
        private string _workingDirectory;
        private string _lowerDirectory = "1234";
        
        // Setup
        public FileDirectoryServiceTests()
        {
            GivenTestFilesDirectoryExists();
            _service = new FileDirectoryService(_workingDirectory);
        }

        [Fact]
        public void GivenRoutingToADirectoryWhichExists_WithALowerAndUpperDirectory_ThenReturnTrue()
        {
            // arrange
            var upperDirectory = Guid.NewGuid().ToString();
            CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);

            // act
            var exists = _service.DirectoryExists(upperDirectory, _lowerDirectory);
            
            // assert
            Assert.True(exists);
            
            // clear down
            DeleteUpperAndLowerDirectories(upperDirectory);
        }

        [Fact]
        public void GivenRoutingToADirectoryWhichDoesNotExist_ReturnFalse()
        {
            // arrange
            // act
            var exists = _service.DirectoryExists("fakeDirectory", _lowerDirectory);
            
            // assert
            Assert.False(exists);
        }
        
        [Theory]
        [InlineData("pdf")]
        [InlineData("html")]
        [InlineData("xls")]
        public async Task GivenAFileExists_OfAnAcceptedType_ReturnFile(string fileType)
        {
            // arrange
            var upperDirectory = Guid.NewGuid().ToString();
            CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);
            CreateTestFile(fileType, upperDirectory);

            // act
            var exists = await _service.FetchFiles(upperDirectory, _lowerDirectory);
            
            // assert
            Assert.Single(exists);
            
            // clear down
            DeleteTestFile(fileType, upperDirectory);
            DeleteUpperAndLowerDirectories(upperDirectory);
        }

        [Fact]
        public async Task GivenAFileExists_OfAnUnAcceptedType_DoNotReturnFile()
        {
            // arrange
            var upperDirectory = Guid.NewGuid().ToString();
            CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);
            CreateTestFile("fake", upperDirectory);

            // act
            var exists = await _service.FetchFiles(upperDirectory, _lowerDirectory);
            
            // assert
            Assert.Empty(exists);
            
            // clear down
            DeleteTestFile("fake",  upperDirectory);
            DeleteUpperAndLowerDirectories(upperDirectory);
        }
        
        [Fact]
        public async Task GivenMultipleFilesExist_OfAcceptedButDifferentTypes_ReturnFiles()
        {
            // arrange
            var upperDirectory = Guid.NewGuid().ToString();
            CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);
            CreateTestFile("pdf", upperDirectory);
            CreateTestFile("html", upperDirectory);
            
            // act
            var exists = await _service.FetchFiles(upperDirectory, _lowerDirectory);
            
            // assert
            Assert.Equal(2, exists.Count());
            
            // clear down
            DeleteTestFile("pdf", upperDirectory);
            DeleteTestFile("html", upperDirectory);
            DeleteUpperAndLowerDirectories(upperDirectory);
        }
        
        [Fact]
        public async Task GivenMultipleVersionsOfFileExist_OfAcceptedType_ReturnOnlyLatestVersionFiles()
        {
            // arrange
            var upperDirectory = Guid.NewGuid().ToString();
            CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);            
            CreateTestFile("112.html", upperDirectory);
            CreateTestFile("115.html", upperDirectory);

            // act
            var exists = await _service.FetchFiles(upperDirectory, _lowerDirectory);
            
            // assert
            Assert.Single(exists);
            Assert.EndsWith("115", exists.First().FileName);
            
            // clear down
            DeleteTestFile("112.html", upperDirectory);
            DeleteTestFile("115.html", upperDirectory);
            DeleteUpperAndLowerDirectories(upperDirectory);
        }
        
        // Spec seems to say  <filename>.<version>.<extension> is expected format so have not covered case of
        //  <filename>.<morefileName>.<version>.<extension> fully. Instead only that if version is provided
        // and it overwrites orginal regardless of format.
        // [Fact]
        // public async Task GivenMultipleVersionsOfFileExist_OneWithVersionInNameOneWithout_ReturnOneWithVersionInTheName()
        // {
        //     // arrange
        //     var upperDirectory = Guid.NewGuid().ToString();
        //     CreateUpperAndLowerDirectories(upperDirectory, _lowerDirectory);
        //     CreateTestFile("01.pdf", upperDirectory);
        //     CreateTestFile("pdf", upperDirectory);
        //
        //     
        //     // act
        //     var exists = await _repository.FetchFiles(upperDirectory, _lowerDirectory);
        //     
        //     // assert
        //     Assert.Single(exists);
        //     Assert.EndsWith("01", exists.First().FileName);
        //     
        //     // clear down
        //     DeleteTestFile("01.pdf", upperDirectory);
        //     DeleteTestFile("pdf", upperDirectory);
        //     DeleteUpperAndLowerDirectories(upperDirectory);
        // }

        private void CreateTestFile(string fileSuffix, string upperDirectory)
        {
            File.Create($"{_workingDirectory}/{upperDirectory}/{_lowerDirectory}/testFile.{fileSuffix}").Dispose();;
        }
        
        private void DeleteTestFile(string fileType, string upperDirectory)
        {
            
            File.Delete($"{_workingDirectory}/{upperDirectory}/{_lowerDirectory}/testFile.{fileType}");
        }

        private void CreateUpperAndLowerDirectories(string upperDirectory, string lowerDirectory)
        {
            Directory.CreateDirectory($"{_workingDirectory}/{upperDirectory}");
            Directory.CreateDirectory($"{_workingDirectory}/{upperDirectory}/{lowerDirectory}");
        }
        
        private void DeleteUpperAndLowerDirectories(string upperDirectory)
        {
            Directory.Delete($"{_workingDirectory}/{upperDirectory}", true);
        }

        private void GivenTestFilesDirectoryExists()
        {
            var getWorkingDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.FullName;
            _workingDirectory = $"{getWorkingDirectory}/TestFiles";
            if (!Directory.Exists(_workingDirectory))
            {
                Directory.CreateDirectory(_workingDirectory);
            }
        }
    }
}