using idee5.Globalization.EFCore;
using idee5.Globalization.Models;
using idee5.Globalization.Queries;
using idee5.Globalization.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace idee5.Globalization.Test
{
    [TestClass]
    public class ResourceRepositoryTest : UnitTestBase
    {
        [TestMethod]
        public async Task ShouldFailToFindLanguage()
        {
            // Arrange
            var qh = new GetParlanceResourcesQueryHandler(repository);

            // Act
            IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesQuery(Constants.CommonTerms, "de-DE", default, default)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 0, actual: items.Count);
        }

        [TestMethod]
        public async Task ShouldFailToFindResourceSet()
        {
            // Arrange
            var qh = new GetParlanceResourcesQueryHandler(repository);

            // Act
            IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesQuery("gargl", "de.DE", default, default)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 0, actual: items.Count);
        }

        [TestMethod]
        public async Task CanFindResourceSet()
        {
            // Arrange
            var qh = new GetParlanceResourcesQueryHandler(repository);

            // Act
            IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesQuery(Constants.CommonTerms, "", default, default)).ConfigureAwait(false);

            // Assert
            Assert.AreNotEqual(notExpected: 0, actual: items.Count);
        }

        [TestMethod]
        public async Task CanFindResourceSetForIndustry()
        {
            // Arrange
            var qh = new GetParlanceResourcesQueryHandler(repository);

            // Act
            IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesQuery(Constants.CommonTerms, "", default, "IT")).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 1, actual: items.Count);
        }

        [TestMethod]
        public async Task CanFindResourceSetForCustomer()
        {
            // Arrange
            var qh = new GetParlanceResourcesQueryHandler(repository);

            // Act
            IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesQuery(Constants.CommonTerms, "", "idee5", default)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 1, actual: items.Count);
        }

        [TestMethod]
        public async Task CanGetAllResources()
        {
            // Arrange
            var qh = new GetAllParlanceResourcesQueryHandler(repository);

            // Act
            IEnumerable<Resource> items = await qh.HandleAsync(new GetAllParlanceResourcesQuery(null,null,null)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 4, actual: items.Count());
            Assert.AreEqual(expected: "Mayhaps", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllResourcesForIndustry()
        {
            // Arrange
            var qh = new GetAllParlanceResourcesQueryHandler(repository);

            // Act
            IEnumerable<Resource> items = await qh.HandleAsync(new GetAllParlanceResourcesQuery(null, null, "IT")).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 4, actual: items.Count());
            Assert.AreEqual(expected: "Mayhaps (Industry)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllResourcesForCustomer()
        {
            // Arrange
            var qh = new GetAllParlanceResourcesQueryHandler(repository);

            // Act
            IEnumerable<Resource> items = await qh.HandleAsync(new GetAllParlanceResourcesQuery(null, "idee5", null)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 4, actual: items.Count());
            Assert.AreEqual(expected: "Mayhaps (Customer)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllResourcesForIndustryAndCustomer()
        {
            // Arrange
            var qh = new GetAllParlanceResourcesQueryHandler(repository);

            // Act
            IEnumerable<Resource> items = await qh.HandleAsync(new GetAllParlanceResourcesQuery(null, "idee5", "IT")).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 4, actual: items.Count());
            Assert.AreEqual(expected: "Mayhaps (Industry + Customer)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllLocalResources()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(true, null, null);
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 2, actual: items.Count());
            Assert.AreEqual(expected: "Test2", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllLocalResourcesForCustomer()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "", Value = "Test2 (Customer)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(true, "idee5", null);
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 2, actual: items.Count());
            Assert.AreEqual(expected: "Test2 (Customer)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllLocalResourcesForIndustry()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Test2 (Industry)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(true, null, "IT");
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 2, actual: items.Count());
            Assert.AreEqual(expected: "Test2 (Industry)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllLocalResourcesForIndustryAndCustomer()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Test2 (Industry)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "Common.Terms", BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "", Value = "Test2 (Industry&Customer)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(true, "idee5", "IT");
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 2, actual: items.Count());
            Assert.AreEqual(expected: "Test2 (Industry&Customer)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanIgnoreGlobalResources()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(true, null, null);
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 0, actual: items.Count());
        }

        [TestMethod]
        public async Task CanGetAllGlobalResources()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(false, null, null);
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 6, actual: items.Count());
            Assert.AreEqual(expected: "Maybee", actual: items.First().Value);
            Assert.AreEqual(expected: "Test2", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllGlobalResourcesForCustomer()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "", Value = "Test2 (Customer)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(false, "idee5", null);
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 6, actual: items.Count());
            Assert.AreEqual(expected: "Maybee (Customer)", actual: items.First().Value);
            Assert.AreEqual(expected: "Test2 (Customer)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllGlobalResourcesForIndustry()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Test2 (Industry)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(false, null, "IT");
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 6, actual: items.Count());
            Assert.AreEqual(expected: "Maybee (Industry)", actual: items.First().Value);
            Assert.AreEqual(expected: "Test2 (Industry)", actual: items.Last().Value);
        }

        [TestMethod]
        public async Task CanGetAllGlobalResourcesForIndustryAndCustomer()
        {
            // Arrange
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Test2" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Test2 (Industry)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Test2", ResourceSet = "GlobalTerms", BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "", Value = "Test2 (Industry&Customer)" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            var query = new GetAllParlanceResourcesQuery(false, "idee5", "IT");
            var handler = new GetAllParlanceResourcesQueryHandler(resourceUnitOfWork.ResourceRepository);

            // Act
            IEnumerable<Resource> items = await handler.HandleAsync(query).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expected: 6, actual: items.Count());
            Assert.AreEqual(expected: "Maybee (Industry + Customer)", actual: items.First().Value);
            Assert.AreEqual(expected: "Test2 (Industry&Customer)", actual: items.Last().Value);
        }
    }
}