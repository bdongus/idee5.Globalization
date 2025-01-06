using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using idee5.Common.Data;
using idee5.Globalization.Commands;
using idee5.Globalization.Models;
using idee5.Globalization.Queries;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Globalization.Test;
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
    public async Task CanImportResxFile() {
        // Arrange
        var handler = new CreateOrUpdateResourceCommandHandler(resourceUnitOfWork);
        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var validationReporter = new ConsoleValidationReporter();
        var inputHandler = new ResxFileInputHandler(new NullLogger<ResxFileInputHandler>());
        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<CreateOrUpdateResourceCommand>(recursiveAnnotationsValidator, validationReporter, handler);

        var importer = new AsyncDataImporter<ResxFileInputHandlerQuery, CreateOrUpdateResourceCommand, NoCleanupCommand>(inputHandler, outputHandler, new NoCleanupCommandHandler());
        var query = new ResxFileInputHandlerQuery("", "", null, null, null) {
            Path = "ImportTest.resx",
            ResourceSet = "testset",
            TargetLanguage = "de"
        };

        // This is needed to support the text file encoding in our resx
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // Act
        await importer.ExecuteAsync(query, new NoCleanupCommand()).ConfigureAwait(false);

        // Assert
        List<Resource> res = await resourceUnitOfWork.ResourceRepository.GetAsync(r => r.ResourceSet == "testset").ConfigureAwait(false);
        Assert.AreEqual(5, res.Count);
        Assert.IsNotNull(res.SingleOrDefault(r => r.Id == "CommentedText")?.Comment);
        Assert.IsNotNull(res.SingleOrDefault(r => r.Id == "Icon1")?.BinFile);
        Assert.IsNotNull(res.SingleOrDefault(r => r.Id == "NewTextFile")?.Textfile);
    }
    [TestMethod]
    public async Task CanImportCultureResxFile() {
        // Arrange
        var handler = new CreateOrUpdateResourceCommandHandler(resourceUnitOfWork);
        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var validationReporter = new ConsoleValidationReporter();
        var inputHandler = new ResxFileInputHandler(new NullLogger<ResxFileInputHandler>());
        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<CreateOrUpdateResourceCommand>(recursiveAnnotationsValidator, validationReporter, handler);

        var importer = new AsyncDataImporter<ResxFileInputHandlerQuery, CreateOrUpdateResourceCommand, NoCleanupCommand>(inputHandler, outputHandler, new NoCleanupCommandHandler());
        var query = new ResxFileInputHandlerQuery("ImportTest.de.resx", "ImportTest", null, null, null);

        // This is needed to support the text file encoding in our resx
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // Act
        await importer.ExecuteAsync(query, new NoCleanupCommand()).ConfigureAwait(false);

        // Assert
        List<Resource> res = await resourceUnitOfWork.ResourceRepository.GetAsync(r => r.ResourceSet == "ImportTest").ConfigureAwait(false);
        Assert.AreEqual(2, res.Count);
        Resource? resource = res.SingleOrDefault(r => r.Id == "CommentedText");
        Assert.IsNotNull(resource?.Comment);
        Assert.AreEqual("de", resource?.Language);
    }
    [TestMethod]
    public async Task DoesNotThrowIfResxNotFound() {
        // Arrange
        var handler = new CreateOrUpdateResourceCommandHandler(resourceUnitOfWork);
        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var validationReporter = new ConsoleValidationReporter();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(o => o.AddConsole());
        var logger = loggerFactory.CreateLogger<ResxFileInputHandler>();
        var inputHandler = new ResxFileInputHandler(logger);
        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<CreateOrUpdateResourceCommand>(recursiveAnnotationsValidator, validationReporter, handler);

        var importer = new AsyncDataImporter<ResxFileInputHandlerQuery, CreateOrUpdateResourceCommand, NoCleanupCommand>(inputHandler, outputHandler, new NoCleanupCommandHandler());
        var query = new ResxFileInputHandlerQuery("", "", null, null, null) {
            Path = "DoesNotExist.resx",
            ResourceSet = "testset",
            TargetLanguage = "de"
        };

        // Act
        await importer.ExecuteAsync(query, new NoCleanupCommand()).ConfigureAwait(false);

        // Assert
        int count = await resourceUnitOfWork.ResourceRepository.CountAsync(r => r.ResourceSet == "testset").ConfigureAwait(false);
        Assert.AreEqual(0, count);
    }
    [TestMethod]
    public async Task DoesNoThrowOnUnavailableFileLink() {
        // Arrange
        var handler = new CreateOrUpdateResourceCommandHandler(resourceUnitOfWork);
        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var validationReporter = new ConsoleValidationReporter();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(o => o.AddConsole());
        var logger = loggerFactory.CreateLogger<ResxFileInputHandler>();
        var inputHandler = new ResxFileInputHandler(logger);
        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<CreateOrUpdateResourceCommand>(recursiveAnnotationsValidator, validationReporter, handler);

        var importer = new AsyncDataImporter<ResxFileInputHandlerQuery, CreateOrUpdateResourceCommand, NoCleanupCommand>(inputHandler, outputHandler, new NoCleanupCommandHandler());
        var query = new ResxFileInputHandlerQuery("", "", null, null, null) {
            Path = "WrongFileLink.resx",
            ResourceSet = "testset",
            TargetLanguage = "de"
        };

        // Act
        await importer.ExecuteAsync(query, new NoCleanupCommand()).ConfigureAwait(false);

        // Assert
        int count = await resourceUnitOfWork.ResourceRepository.CountAsync(r => r.ResourceSet == "testset").ConfigureAwait(false);
        Assert.AreEqual(1, count);
    }
    [TestMethod]
    public async Task CanImportAssemblyResources() {
        // Arrange
        var handler = new CreateOrUpdateResourceCommandHandler(resourceUnitOfWork);
        var recursiveAnnotationsValidator = new RecursiveAnnotationsValidator();
        var validationReporter = new ConsoleValidationReporter();
        var inputHandler = new ResourceAssemblyInputHandler(new NullLogger<ResourceAssemblyInputHandler>());
        var outputHandler = new DataAnnotationValidationCommandHandlerAsync<CreateOrUpdateResourceCommand>(recursiveAnnotationsValidator, validationReporter, handler);

        var importer = new AsyncDataImporter<ResourceAssemblyInputHandlerQuery, CreateOrUpdateResourceCommand, NoCleanupCommand>(inputHandler, outputHandler, new NoCleanupCommandHandler());
        var query = new ResourceAssemblyInputHandlerQuery(".\\de\\idee5.Globalization.Test.resources.dll", null, null, null);

        // Act
        await importer.ExecuteAsync(query, new NoCleanupCommand()).ConfigureAwait(false);

        // Assert
        List<Resource> res = await resourceUnitOfWork.ResourceRepository.GetAsync(r => r.ResourceSet == "idee5.Globalization.Test.Properties.Resources").ConfigureAwait(false);
        Assert.AreEqual(2, res.Count);
        Assert.IsNull(res.SingleOrDefault(r => r.Id == "CommentedText")?.Comment);

    }
}
