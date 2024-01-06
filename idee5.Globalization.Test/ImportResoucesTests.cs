using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using idee5.Common.Data;
using idee5.Globalization.Commands;
using idee5.Globalization.Models;
using idee5.Globalization.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Globalization.Test {
    [TestClass]
    public class ImportResoucesTests : WithSQLiteBase {
        [TestMethod]
        public async Task CanImportResources() {
            // Arrange
            var resourcesToImport = new Resource[] {
                // update existing resource (value changed)
                new() { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "de-CH", Value = "Villücht (Branche + Kunde)" },
                // insert simple resource
                new() { Id = "xyz", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de-CH", Value = "xyz" },
                // insert textfile
                new() { Id = "textfile", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "app.config" },
                // insert binfile
                new() { Id = "binfile", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "New.png" },
                // same as existing resource
                new() { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" }
            };
            var cmd = new ImportResourcesCommand([]) {
                Resources = resourcesToImport
            };
            var handler = new ImportResourcesCommandHandler(resourceUnitOfWork);
            var startCount = await context.Resources.CountAsync().ConfigureAwait(false);
            // Act
            await handler.HandleAsync(cmd).ConfigureAwait(false);
            var endCount = await context.Resources.CountAsync().ConfigureAwait(false);
            // Assert
            Assert.AreEqual(3, endCount - startCount);
            Resource? maybeCH = await resourceUnitOfWork.ResourceRepository.GetSingleAsync(r => r.Id == "Maybe" && r.ResourceSet == Constants.CommonTerms && r.Customer == "idee5" && r.Language == "de-CH" && r.Industry == "IT").ConfigureAwait(false);
            Assert.AreEqual("Villücht (Branche + Kunde)", maybeCH?.Value);
        }

        [TestMethod]
        public async Task CanImportResxFileAsync() {
            // Arrange
            var handler = new ImportResourcesCommandHandler(resourceUnitOfWork);
            var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
            var validationReporter = new ConsoleValidationReporter();
            var inputHandler = new ResxFileInputHandler();
            var outputHandler = new DataAnnotationValidationCommandHandlerAsync<ImportResourcesCommand>(recursiveAnnotationsValidator, validationReporter, handler);
            var converter = new DataConverterAsync<ResxFileInputHandlerQuery, ImportResourcesCommand>(inputHandler, outputHandler);
            var query = new ResxFileInputHandlerQuery("", "", null, null, null) {
                Path = "ImportTes.resx",
                ResourceSet = "testset",
                TargetLanguage = "de"
            };

            // Act
            await converter.ExecuteAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            var count = await resourceUnitOfWork.ResourceRepository.CountAsync(r => r.ResourceSet == "testset").ConfigureAwait(false);
            Assert.AreNotEqual(5, count);
        }
    }
}
