using idee5.Globalization.EFCore;
using idee5.Globalization.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test {
    [TestClass]
    public class QueryWithSQLiteTests : WithSQLiteBase {
        [TestMethod]
        public async Task CanHandleParlanceWithFallbackQuery() {
            // Arrange
            var qh = new GetParlanceResourcesWithFallbackQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IDictionary<string, object> result = await qh.HandleAsync(new GetParlanceResourcesWithFallbackQuery(Constants.CommonTerms, "de-LI", "idee5", "IT"), CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 1, actual: result.Count);
            // "de-LI" does not exists, so it should be the "de" versions 
            Assert.AreEqual("Vielleicht (Branche + Kunde)", result["Maybe"]);
        }
        [TestMethod]
        public async Task CanGetResourceKeysForResoureSet() {
            // Arrange
            var qh = new GetResourceKeysForResourceSetQueryHandler(context);

            // Act
            var query = new GetResourceKeysForResourceSetQuery(Constants.CommonTerms);
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }
        [TestMethod]
        public async Task CanFindResourceKeysInResourceSetByValue() {
            // Arrange
            var qh = new SearchResourceKeysForResourceSetQueryHandler(context);

            // Act
            var query = new SearchResourceKeysForResourceSetQuery(Constants.CommonTerms, "May");
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }
        [TestMethod]
        public async Task CanFindResourceKeysInResourceSetByCustomer() {
            // Arrange
            var qh = new SearchResourceKeysForResourceSetQueryHandler(context);

            // Act
            var query = new SearchResourceKeysForResourceSetQuery(Constants.CommonTerms, "idee5");
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task CanFindAllResourceKeysInResourceSet() {
            // Arrange
            var qh = new SearchResourceKeysForResourceSetQueryHandler(context);

            // Act
            var query = new SearchResourceKeysForResourceSetQuery(Constants.CommonTerms);
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task CanFindValueInResourceKeys() {
            // Arrange
            var qh = new SearchResourceKeysQueryHandler(context);

            // Act
            var query = new SearchResourceKeysQuery("May");
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task CanFindAllResourceKeys() {
            // Arrange
            var qh = new SearchResourceKeysQueryHandler(context);

            // Act
            var query = new SearchResourceKeysQuery();
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task CanFindResourceSetInResourceKeys() {
            // Arrange
            var qh = new SearchResourceKeysQueryHandler(context);

            // Act
            var query = new SearchResourceKeysQuery("Terms");
            var result = await qh.HandleAsync(query, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(4, result.Count);
        }
    }
}