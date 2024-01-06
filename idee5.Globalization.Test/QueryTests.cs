using idee5.Globalization.Models;
using idee5.Globalization.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test {
    [TestClass]
    public class QueryTests : UnitTestBase {
        [TestInitialize]
        public void QueryTestInitialize() {
            if (!context.Resources.Any(r => r.ResourceSet == "CommonTerms2")) {
                resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = "CommonTerms2", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" });
                resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = "CommonTerms3", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "Mayhaps" });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public async Task CanHandleResourceSetQuery() {
            var qh = new GetResourceSetsByNameQueryHandler(queryRepository);
            IList<string> result = await qh.HandleAsync(new GetResourceSetsByNameQuery("") { Name = "Term" }, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(expected: 3, actual: result.Count);
        }

        [TestMethod]
        public async Task CanHandleEmptyResourceSetQuery() {
            var qh = new GetResourceSetsByNameQueryHandler(queryRepository);
            IList<string> result = await qh.HandleAsync(new GetResourceSetsByNameQuery("") { Name = "" }, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(expected: 3, actual: result.Count);
        }

        [TestMethod]
        public async Task CanHandleResourcesForResourceSetQuery() {
            var qh = new GetAllResourcesForResourceSetQueryHandler(queryRepository);
            IList<Resource> result = await qh.HandleAsync(new GetAllResourcesForResourceSetQuery("") { ResourceSet = Constants.CommonTerms }, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(expected: 16, actual: result.Count);
        }

        [TestMethod]
        public async Task CanGetExactResource() {
            // Arrange
            var q = new GetExactResourceQuery("", "", null, null, null) { ResourceSet = Constants.CommonTerms, Id = "Maybe", Customer = string.Empty, Industry = string.Empty, Language = "en-GB" };
            var qh = new GetExactResourceQueryHandler(queryRepository);

            // Act
            Resource? result = await qh.HandleAsync(q).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("en-GB", result.Language);
        }

        [TestMethod]
        public async Task ReturnsNullIfNoExactResourceFound() {
            // Arrange
            var q = new GetExactResourceQuery("", "", null, null, null) { ResourceSet = Constants.CommonTerms, Id = "Maybe", Customer = string.Empty, Industry = string.Empty, Language = "abc" };
            var qh = new GetExactResourceQueryHandler(queryRepository);

            // Act
            Resource? result = await qh.HandleAsync(q).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }
    }
}