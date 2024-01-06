using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using idee5.Globalization.Repositories;
using System.Collections.Generic;
using idee5.Globalization.Models;
using idee5.Globalization.EFCore;
using System.Threading.Tasks;
using System.Globalization;

namespace idee5.Globalization.Test {
    class LocalizedEnumConverter : ResourceEnumConverter {
        public static IResourceRepository Repository;
        public static IList<Resource> ExpectedResources = new List<Resource>();
        public LocalizedEnumConverter(Type type) : base(type, GetResourceManager()) { }

        public static DatabaseResourceManager GetResourceManager() {
            Repository = new ResourceRepository(UnitTestBase.context);
            return new DatabaseResourceManager(Repository, resourceSet: "Enums", industry: "", customer: "");
        }
    }

    [TestClass]
    public class ResourceEnumConverterTest : UnitTestBase {
        [TypeConverter(typeof(LocalizedEnumConverter))]
        private enum TestEnum {
            Started,
            Stopped,
            Failed
        }

        [TestMethod]
        public async Task CanFindStringResourceWithCulture() {
            // Arrange
            var language = CultureInfo.CurrentCulture.IetfLanguageTag;
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "TestEnum_Stopped", ResourceSet = "Enums", BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = language, Value = "Gestoppt" });
            await resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            // Act
            var result = TypeDescriptor.GetConverter(TestEnum.Stopped).ConvertTo(TestEnum.Stopped, typeof(string));

            // Assert

            Assert.AreEqual(expected: "Gestoppt", actual: result);
        }
    }
}
