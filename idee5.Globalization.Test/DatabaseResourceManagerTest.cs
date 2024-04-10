using System.Globalization;
using System.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Globalization.Test
{
    [TestClass]
    public class DatabaseResourceManagerTest : UnitTestBase {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanFindStringResourceWithCulture() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-CH"));
            Assert.AreEqual(expected: "Villicht", actual: result);
        }

        [TestMethod]
        public void CanFindStringResourceWithInvariantCulture() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "en-US"));
            Assert.AreEqual(expected: "Maybee", actual: result);
        }

        [TestMethod]
        public void CanFindStringResourceWithIndustry() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "IT", customer: "");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-CH"));
            Assert.AreEqual(expected: "Villicht (Branche)", actual: result);
        }

        [TestMethod]
        public void CanFindStringResourceWithIndustryAndCustomer() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "IT", customer: "idee5");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-CH"));
            Assert.AreEqual(expected: "Villicht (Branche + Kunde)", actual: result);
        }

        [TestMethod]
        public void CanFindStringResourceWithCustomer() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: null, customer: "idee5");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-CH"));
            Assert.AreEqual(expected: "Villicht (Kunde)", actual: result);
        }

        [TestMethod]
        public void CanFindStringResourceWithShortCulture() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Maybe", culture: new CultureInfo(name: "de-AT"));
            Assert.AreEqual(expected: "Vielleicht", actual: result);
        }

        [TestMethod]
        public void ShouldFailMissingResourceSet() {
            var rm = new DatabaseResourceManager(repository, resourceSet: "CommonTerm", industry: "", customer: "");
            string?  result = rm.GetString(name: "Maybe");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldFailMissingResource() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            string? result = rm.GetString(name: "Hello");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanFindResourceSet() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            ResourceSet? result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);
            result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Villicht", actual: result.GetString(name: "Maybe"));
        }

        [TestMethod]
        public void CanFindResourceSetFromCache() {
            var rm = new DatabaseResourceManager(repository, resourceSet: Constants.CommonTerms, industry: "", customer: "");
            ResourceSet? result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);
            result = rm.GetResourceSet(new CultureInfo(name: "de-CH"), createIfNotExists: true, tryParents: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Villicht", actual: result.GetString(name: "Maybe"));
        }
    }
}
