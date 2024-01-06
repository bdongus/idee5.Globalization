using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using idee5.Common.Data;
using idee5.Globalization.Models;
using System.Threading.Tasks;
using NSpecifications;

namespace idee5.Globalization.Test {
    [TestClass]
    public class DatabaseResourceWriterWithSQLiteTest : WithSQLiteBase {
        [TestMethod]
        public async Task CanWriteStringResource() {
            // Arrange
            var dbw = new DatabaseResourceWriter(resourceUnitOfWork, resourceSet: Constants.CommonTerms, language: "", industry: "", customer: "");
            dbw.AddResource(name: "Gargl", value: "Tchaka");
            dbw.Generate();

            // Act
            ASpec<Resource> query = Specifications.InResourceSet(Constants.CommonTerms) & Specifications.ResourceId("Gargl") & new Spec<Resource>(r => r.Language == "" && r.Industry == "" && r.Customer == "");
            Resource? result = await resourceUnitOfWork.ResourceRepository.GetSingleAsync(query).ConfigureAwait(false);
            dbw.Dispose();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected: "Tchaka", actual: result.Value);
        }
    }
}
