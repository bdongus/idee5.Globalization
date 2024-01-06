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
    }
}