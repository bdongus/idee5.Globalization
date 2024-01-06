using idee5.Common;
using idee5.Common.Data;
using idee5.Globalization.EFCore;
using idee5.Globalization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace idee5.Globalization.Test {
    [TestClass]
    public class WithSQLiteBase : IDisposable {
        protected GlobalizationDbContext context;
        protected ResourceUnitOfWork resourceUnitOfWork;

        [TestInitialize]
        public void MyTestInitalize() {
            var contextOptions = new DbContextOptionsBuilder<GlobalizationDbContext>();
            // work around for SQLite provider not resolving the placeholder
            var fileName = "|DataDirectory|idee5.Resources.db3".ReplaceDataDirectory();

            contextOptions.UseSqlite("data source=" + fileName);
            contextOptions.EnableSensitiveDataLogging();
            context = new GlobalizationDbContext(contextOptions.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            resourceUnitOfWork = new ResourceUnitOfWork(context);

            // Add the test data
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "Mayhaps" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Vielleicht" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de-CH", Value = "Villicht" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Maybee (Industry)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "en-GB", Value = "Mayhaps (Industry)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "de", Value = "Vielleicht (Branche)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "de-CH", Value = "Villicht (Branche)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "", Value = "Maybee (Customer)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "en-GB", Value = "Mayhaps (Customer)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "de", Value = "Vielleicht (Kunde)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "de-CH", Value = "Villicht (Kunde)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "", Value = "Maybee (Industry + Customer)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "en-GB", Value = "Mayhaps (Industry + Customer" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "de", Value = "Vielleicht (Branche + Kunde)" });
            resourceUnitOfWork.ResourceRepository.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "de-CH", Value = "Villicht (Branche + Kunde)" });
            resourceUnitOfWork.SaveChangesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        #region IDisposable Support
        private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    resourceUnitOfWork.Dispose();
                    context.Dispose();
                }

                _disposedValue = true;
            }
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose() {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}