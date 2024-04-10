using idee5.Globalization.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSpecifications;

using System.Globalization;
using System.Resources;
using System.Threading.Tasks;

using static idee5.Globalization.Specifications;

namespace idee5.Globalization.Test {
    [TestClass]
    public class DatabaseResourceManagerWithSQLiteTest : WithSQLiteBase {
        [TestMethod]
        public void CanFindStringResourceWithShortCulture() {
            var rm = new DatabaseResourceManager(resourceUnitOfWork.ResourceRepository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-AT"));
            Assert.AreEqual(expected: "Vielleicht", actual: result);
        }

        [TestMethod]
        public void ShouldFailMissingResourceSet() {
            var rm = new DatabaseResourceManager(resourceUnitOfWork.ResourceRepository, resourceSet: "CommonTerm", industry: "", customer: "");
            string? result = rm.GetString(name: "Maybe");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldFailMissingResource() {
            var rm = new DatabaseResourceManager(resourceUnitOfWork.ResourceRepository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Hello");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanFindResourceSet() {
            var rm = new DatabaseResourceManager(resourceUnitOfWork.ResourceRepository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            ResourceSet? result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Villicht", actual: result.GetString(name: "Maybe"));
        }

        [TestMethod]
        public void CanFindResourceSetFromCache() {
            var rm = new DatabaseResourceManager(resourceUnitOfWork.ResourceRepository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            _ = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);
            ResourceSet? result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Villicht", actual: result.GetString(name: "Maybe"));
        }

        [TestMethod]
        public async Task CanFindResource() {
            // Arrange
            ASpec<Resource> maybe = ResourceId("Maybe") & InResourceSet(Constants.CommonTerms) & OfLanguage("de-CH") & CustomerNeutral & IndustryNeutral;

            // Act
            Resource? result = await resourceUnitOfWork.ResourceRepository.GetSingleAsync(maybe).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Villicht", actual: result.Value);
        }
    }
}