using idee5.Globalization.Configuration;
using idee5.Globalization.EFCore;
using idee5.Globalization.Models;
using idee5.Globalization.Queries;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test;
[TestClass]
public class LocalizerFactoryTests : UnitTestBase {
    [TestMethod]
    public async Task CanGetStringForTypeFactory() {
        // Arrange
        resourceUnitOfWork.ResourceRepository.Add(new Resource() {
            ResourceSet = typeof(LocalizerFactoryTests).FullName!,
            Id = "Value1",
            Language = "de",
            Value = "Test",
            Customer = "",
            Industry = ""
        });
        await resourceUnitOfWork.SaveChangesAsync();
        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);

        var factory = new EFCoreStringLocalizerFactory(options, new ContextFactory(this));
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // Act
        var localizer = factory.Create(typeof(LocalizerFactoryTests));
        var result = localizer["Value1"];

        // Assert
        Assert.AreEqual("Test", result.Value);
    }

    [TestMethod]
    public async Task CanGetStringForStringFactory() {
        // Arrange
        Type type = typeof(LocalizerFactoryTests);
        resourceUnitOfWork.ResourceRepository.Add(new Resource() {
            ResourceSet = type.FullName!,
            Id = "Value2",
            Language = "de",
            Value = "Test",
            Customer = "",
            Industry = ""
        });
        await resourceUnitOfWork.SaveChangesAsync();
        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var factory = new EFCoreStringLocalizerFactory(options, new ContextFactory(this));
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // Act
        var localizer = factory.Create(type.Name, new AssemblyName(type.Assembly.FullName!).Name!);
        var result = localizer["Value2"];

        // Assert
        Assert.AreEqual("Test", result.Value);
    }

    [TestMethod]
    public async Task CanGetStringForCustomResourceSet() {
        // Arrange
        resourceUnitOfWork.ResourceRepository.Add(new Resource() {
            ResourceSet = "CommonTerms",
            Id = "Value2",
            Language = "de",
            Value = "Test",
            Customer = "",
            Industry = ""
        });
        await resourceUnitOfWork.SaveChangesAsync();
        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var factory = new EFCoreStringLocalizerFactory(options, new ContextFactory(this));
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // Act
        var localizer = factory.Create("CommonTerms", "");
        var result = localizer["Value2"];

        // Assert
        Assert.AreEqual("Test", result.Value);
    }

    [TestMethod]
    public void ReturnsLocalizerFromCache() {
        // Arrange
        Type type = typeof(LocalizerFactoryTests);

        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var factory = new EFCoreStringLocalizerFactory(options, new ContextFactory(this));
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // Act
        var localizer1 = factory.Create(type.Name, new AssemblyName(type.Assembly.FullName!).Name!);
        var localizer2 = factory.Create(type.Name, new AssemblyName(type.Assembly.FullName!).Name!);

        // Assert
        Assert.AreEqual(localizer1, localizer2);
    }

    [TestMethod]
    public void ReturnsSameLocalizerFromTypeAndString() {
        // Arrange
        Type type = typeof(LocalizerFactoryTests);

        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var factory = new EFCoreStringLocalizerFactory(options, new ContextFactory(this));
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // Act
        var localizer1 = factory.Create(type);
        var localizer2 = factory.Create(type.Name, new AssemblyName(type.Assembly.FullName!).Name!);

        // Assert
        Assert.AreEqual(localizer1, localizer2);
    }
}