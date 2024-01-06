using idee5.Globalization.Configuration;
using idee5.Globalization.EFCore;
using idee5.Globalization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test;
[TestClass]
public class ServiceCollectionTests : UnitTestBase{
    [TestMethod]
    public async Task CanAddEFCoreServices() {
        // Arrange
        var services = new ServiceCollection();
        var cultureInfo = new System.Globalization.CultureInfo("de-CH");
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        resourceUnitOfWork.ResourceRepository.Add(new Resource() {
            ResourceSet = typeof(ServiceCollectionTests).FullName!,
            Id = "Value1",
            Language = "de",
            Value = "Test",
            Customer = "",
            Industry = ""
        });
        await resourceUnitOfWork.SaveChangesAsync();

        services.Configure<LocalizationParlanceOptions>(o => {
            o.Customer = null;
            o.Industry = null;
        });

        // Act
        services.AddEFCoreLocalization(o => o.UseSqlite(_connection));
        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IStringLocalizerFactory>();

        LocalizedString? result = null;
        if (factory != null) {
            IStringLocalizer localizer = factory.Create(typeof(ServiceCollectionTests));
            result = localizer["Value1"];
        }

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Value);
    }
}
