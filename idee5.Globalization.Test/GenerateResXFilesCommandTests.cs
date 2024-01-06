using idee5.Globalization.Commands;
using idee5.Globalization.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace idee5.Globalization.Test {
    [TestClass]
    public class GenerateResXFilesCommandTests : UnitTestBase {
        private const string _filePath = @"c:\temp\resources";

        [TestMethod]
        public async Task CanGenerateGlobalResXFiles() {
            // Arrange
            const string globalresxpath = _filePath + @"\globalresx";
            var command = new GenerateResXFilesCommand(default, null, null, null) { BasePhysicalPath = globalresxpath, LocalResources = false };
            var handler = new GenerateResXFilesCommandHandler(resourceUnitOfWork);

            // Act
            await handler.HandleAsync(command).ConfigureAwait(false);
            string[] fileList = Directory.GetFiles(globalresxpath);

            // Assert
            Assert.AreEqual(4, fileList.Length);
            //TODO: Validate resx files.
        }

        [TestMethod]
        public async Task CanGenerateLocalResXFiles() {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "", Value = "Test2 (Customer)" });
            await resourceUnitOfWork.SaveChangesAsync();
            const string localresxpath = _filePath + @"\localresx";
            var command = new GenerateResXFilesCommand(default, null, null, null) { BasePhysicalPath = localresxpath, LocalResources = true };
            var handler = new GenerateResXFilesCommandHandler(resourceUnitOfWork);

            // Act
            await handler.HandleAsync(command).ConfigureAwait(false);

            // Assert
            FileInfo[] list = new DirectoryInfo(new FileInfo("Common.Terms".GenerateResourceSetPath(localresxpath, true)).DirectoryName).GetFiles();
            Assert.AreEqual(expected: 1, actual: list.Length);
            Assert.AreEqual(expected: "Common.Terms.resx", actual: list[0].Name);
        }
    }
}