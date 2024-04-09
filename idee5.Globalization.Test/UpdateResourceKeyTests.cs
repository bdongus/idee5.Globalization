﻿using idee5.Globalization.Commands;
using idee5.Globalization.Models;

using MELT;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test;

[TestClass]
public class UpdateResourceKeyTests : UnitTestBase {
    private const string _resourceSet = "UpdateResourceKey";

    [TestInitialize]
    public void TestInitialize() {
        if (!context.Resources.Any(Specifications.InResourceSet(_resourceSet))) {
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "DeRemove", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "xx" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "DeRemove", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "xxx" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "LogTest", ResourceSet = _resourceSet, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "xxx" });
            context.SaveChanges();
        }
    }

    [TestMethod]
    public async Task CanCreateUpdateAndDeleteTranslation() {
        // Arrange
        var handler = new UpdateResourceKeyCommandHandler(resourceUnitOfWork, new NullLogger<UpdateResourceKeyCommandHandler>());
        var rk = new ResourceKey() { ResourceSet = _resourceSet, Id ="DeRemove" };
        var translations = new Dictionary<string, string> {
            { "en-GB", "lsmf" },
            { "it", "xyz" }
        };
        var command = new UpdateResourceKeyCommand(rk, translations.ToImmutableDictionary());

        // Act
        await handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var rsc = await resourceUnitOfWork.ResourceRepository.GetAsync(r => r.ResourceSet == _resourceSet).ConfigureAwait(false);
        Assert.AreEqual(2, rsc.Count);
        Assert.AreEqual("lsmf", rsc.SingleOrDefault(r => r.Language == "en-GB")?.Value);
        Assert.AreEqual("xyz", rsc.SingleOrDefault(r => r.Language == "it")?.Value);
    }

    [TestMethod]
    public async Task CanCreateLogs() {
        // Arrange
        var loggerFactory = TestLoggerFactory.Create();
        var logger = loggerFactory.CreateLogger<UpdateResourceKeyCommandHandler>();
        var handler = new UpdateResourceKeyCommandHandler(resourceUnitOfWork, logger);
        var rk = new ResourceKey() { ResourceSet = _resourceSet, Id ="LogTest" };
        var translations = new Dictionary<string, string> {
            { "en-GB", "lsmf" },
            { "it", "xyz" }
        };
        var command = new UpdateResourceKeyCommand(rk, translations.ToImmutableDictionary());

        // Act
        await handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);


        // Assert
        Assert.AreEqual(4, loggerFactory.Sink.LogEntries.Count());
        // 2 create/update events
        Assert.AreEqual(2, loggerFactory.Sink.LogEntries.Count(le => le.EventId.Id == 4));
        Assert.AreEqual(2, loggerFactory.Sink.LogEntries.Single(le =>le.EventId == 2).Properties.Single(p => p.Key == "Count").Value);
    }
}