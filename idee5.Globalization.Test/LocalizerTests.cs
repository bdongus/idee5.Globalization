using idee5.Globalization.Configuration;
using idee5.Globalization.EFCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace idee5.Globalization.Test;
[TestClass]
public class LocalizerTests : UnitTestBase {
    [TestMethod]
    public void CanGetString() {
        // Arrange
        var locOpt = new LocalizationParlanceOptions();
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var rm = new DatabaseResourceManager(repository, Constants.CommonTerms, options.Value.Industry, options.Value.Customer);

        var localizer = new EFCoreStringLocalizer(new ContextFactory(this), rm, options);
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        // Act
        var result = localizer["Maybe"];

        // Assert
        Assert.AreEqual(expected: "Villicht", actual: result);
    }

    [TestMethod]
    public void CanGetStringWithIndustry() {
        // Arrange
        var locOpt = new LocalizationParlanceOptions() { Industry = "IT" };
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var rm = new DatabaseResourceManager(repository, Constants.CommonTerms, options.Value.Industry, options.Value.Customer);

        var localizer = new EFCoreStringLocalizer(new ContextFactory(this), rm, options);
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        // Act
        var result = localizer["Maybe"];

        // Assert
        Assert.AreEqual(expected: "Villicht (Branche)", actual: result);
    }

    [TestMethod]
    public void CanGetStringWithCustomer() {
        // Arrange
        var locOpt = new LocalizationParlanceOptions() { Customer = "idee5" };
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var rm = new DatabaseResourceManager(repository, Constants.CommonTerms, options.Value.Industry, options.Value.Customer);

        var localizer = new EFCoreStringLocalizer(new ContextFactory(this), rm, options);
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        // Act
        var result = localizer["Maybe"];

        // Assert
        Assert.AreEqual(expected: "Villicht (Kunde)", actual: result);
    }

    [TestMethod]
    public void CanGetStringWithCustomerAndIndustry() {
        // Arrange
        var locOpt = new LocalizationParlanceOptions() { Customer = "idee5", Industry = "IT" };
        IOptions<LocalizationParlanceOptions> options = Options.Create(locOpt);
        var rm = new DatabaseResourceManager(repository, Constants.CommonTerms, options.Value.Industry, options.Value.Customer);

        var localizer = new EFCoreStringLocalizer(new ContextFactory(this), rm, options);
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        // Act
        var result = localizer["Maybe"];

        // Assert
        Assert.AreEqual(expected: "Villicht (Branche + Kunde)", actual: result);
    }
}
