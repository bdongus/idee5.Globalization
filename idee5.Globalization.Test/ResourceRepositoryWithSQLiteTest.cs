using idee5.Common.Data;
using idee5.Globalization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSpecifications;
using System.Linq;
using System.Threading.Tasks;
using static idee5.Globalization.Specifications;

namespace idee5.Globalization.Test
{
    [TestClass]
    public class ResourceRepositoryWithSQLiteTest : WithSQLiteBase
    {
        [TestMethod]
        public async Task CanAddAndFindNewResource()
        {
            var newRes = new Resource { Id = "New", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Nu" };
            resourceUnitOfWork.ResourceRepository.Add(newRes);
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            ASpec<Resource> testSpec = ResourceId("New") & InResourceSet(Constants.CommonTerms) & NeutralLanguage & CustomerNeutral & IndustryNeutral;
            Resource result = await resourceUnitOfWork.ResourceRepository.GetSingleAsync(testSpec)
                .ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Nu", actual: result.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void ShouldFailToAddExistingResource()
        {
            var newRes = new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" };
            resourceUnitOfWork.ResourceRepository.Add(newRes);
        }

        [TestMethod]
        public async Task CanRemoveResourceAsync()
        {
            // Arrange
            var toBeRemoved = new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" };
            var startCount = await context.Resources.CountAsync().ConfigureAwait(false);

            // Act
            resourceUnitOfWork.ResourceRepository.Remove(toBeRemoved);
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            // Assert
            var endCount = await context.Resources.CountAsync().ConfigureAwait(false);
            Assert.AreEqual(-1, endCount - startCount);
        }
    }
}