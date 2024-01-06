using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Globalization.Test {
    [TestClass]
    public class CultureHelperTest {
        [TestMethod]
        public void CanFindDefaultCulture() {
            string culture = CultureHelper.Instance.GetDefaultCulture();
            Assert.AreEqual(expected: "de-DE", actual: culture);
        }

        [TestMethod]
        public void CanHandleUnimplementedCulture() {
            string culture = CultureHelper.Instance.GetImplementedCulture(name: "es-ES");
            Assert.AreEqual(expected: "de-DE", actual: culture);
        }

        [TestMethod]
        public void CanHandleUnimplementedSubLanguage() {
            string culture = CultureHelper.Instance.GetImplementedCulture(name: "de-CH");
            // if the language isn't implemented it returns the closest match
            Assert.AreEqual(expected: "de-DE", actual: culture);
        }
        [TestMethod]
        public void CanHandleInvalidLanguge() {
            string culture = CultureHelper.Instance.GetImplementedCulture(name: "xx");
            // if the language isn't implemented it returns the closest match
            Assert.AreEqual(expected: "de-DE", actual: culture);
        }
    }
}