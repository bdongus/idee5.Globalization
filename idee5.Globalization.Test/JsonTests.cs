using idee5.Globalization.Models;
using idee5.Globalization.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Test {
    [TestClass]
    public class JsonTests : UnitTestBase {
        [TestMethod]
        public async Task CanGetAllResourcesForResourceSetQueryAsync() {
            var qh = new GetAllJsonResourcesForResourceSetQueryHandler(repository);
            string result = await qh.HandleAsync(new GetAllJsonResourcesForResourceSetQuery(Constants.CommonTerms), CancellationToken.None).ConfigureAwait(false);
            var deserialized = (Resource[]?)JsonSerializer.Deserialize(result, typeof(Resource[]));
            Assert.AreEqual(expected: 16, actual: deserialized?.Length);
        }

        [TestMethod]
        public async Task CanGetNestedJsonResourcesAsync() {
            // Arrange
            const string expected = "{\"Level1\":{\"Level2\":{\"Level3\":{\"Prop1\":\"Test\",\"Prop2\":\"Test\"},\"Prop1\":\"Test\"},\"Prop1\":\"Test\"},\"Maybe\":\"Vielleicht\"}";
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Level1.Prop1", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Level1.Level2.Level3.Prop1", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Level1.Level2.Level3.Prop2", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Level1.Level2.Prop1", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Test" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var qh = new GetNestedJsonParlanceResourcesQueryHandler(repository);

            // Act
            var query = new GetNestedJsonParlanceResourcesQuery(Constants.CommonTerms, "de", default, default);
            string result = await qh.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}