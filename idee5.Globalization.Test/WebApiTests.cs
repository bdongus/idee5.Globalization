using idee5.Globalization.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace idee5.Globalization.Test;
[TestClass]
public class WebApiTests {
    [TestMethod]
    public async Task CanReadResourceTranslations() {
        // Arrange
        await using var webapp = new WebApplicationFactory<Web.Startup>();
        using HttpClient client = webapp.CreateClient();

        // Act
        var response = await client.GetFromJsonAsync<ResourceTranslations>("/api/query/GetResourceKeyTranslations?ResourceSet=CommonTerms&Id=Maybe");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual("CommonTerms", response.ResourceSet);
        Assert.AreEqual(3, response.Translations.Length);
    }
}
