using idee5.Globalization.Commands;
using idee5.Globalization.Models;

using MELT;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test;

[TestClass]
public class DeleteResourceKeyTests : UnitTestBase {
    private const string _resourceSet = "DeleteResourceKey";

    [TestInitialize]
    public void TestInitialize() {
        if (!context.Resources.Any(Specifications.InResourceSet(_resourceSet))) {
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "ToBeDeleted", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "xx" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "ToBeDeleted", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "xxx" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "ToBeDeleted", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "en-GB", Value = "xxx" });
            context.SaveChanges();
        }
    }

    [TestMethod]
    public async Task CanDeleteResourceKey() {
        // Arrange
        var loggerFactory = TestLoggerFactory.Create();
        var logger = loggerFactory.CreateLogger<DeleteResourceKeyCommandHandler>();
        var handler = new DeleteResourceKeyCommandHandler(resourceUnitOfWork, logger);

        var command = new DeleteResourceKeyCommand() { ResourceSet = _resourceSet, Id = "ToBeDeleted" };

        // Act
        await handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var rsc = await resourceUnitOfWork.ResourceRepository.GetAsync(r => r.ResourceSet == _resourceSet && r.Id == "ToBeDeleted").ConfigureAwait(false);
        // the customer specific key still exists
        Assert.AreEqual(1, rsc.Count);
        Assert.AreEqual(1, loggerFactory.Sink.LogEntries.Count());
        Assert.AreEqual(1, loggerFactory.Sink.LogEntries.Count(le => le.EventId.Id == 5));
    }
}