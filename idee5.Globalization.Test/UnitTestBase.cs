using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using idee5.Globalization.Repositories;
using idee5.Globalization.Models;
using idee5.Globalization.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace idee5.Globalization.Test {
    /// <summary>
    /// Init and cleanup for all tests.
    /// </summary>
    [TestClass]
    public class UnitTestBase : IDisposable {
        public static GlobalizationDbContext context;
        protected IResourceRepository repository;
        protected IResourceQueryRepository queryRepository;
        protected IResourceUnitOfWork resourceUnitOfWork;

        internal SqliteConnection _connection;

        internal class ContextFactory : IDbContextFactory<GlobalizationDbContext> {
            private readonly UnitTestBase _parent;

            public ContextFactory(UnitTestBase parent)
            {
                _parent = parent;
            }
            public GlobalizationDbContext CreateDbContext() {
                var contextOptions = new DbContextOptionsBuilder<GlobalizationDbContext>();
                contextOptions.UseSqlite(_parent._connection);
                contextOptions.EnableSensitiveDataLogging();
                return new GlobalizationDbContext(contextOptions.Options);
            }
        }
        [TestInitialize]
        public void MyTestInitialize() {
            var contextOptions = new DbContextOptionsBuilder<GlobalizationDbContext>();
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            contextOptions.UseSqlite(_connection);
            contextOptions.EnableSensitiveDataLogging();
            context = new GlobalizationDbContext(contextOptions.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            repository = new ResourceRepository(context);
            queryRepository = new ResourceQueryRepository(context);
            resourceUnitOfWork = new ResourceUnitOfWork(context);

            // fill the "database"

            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "", Value = "Maybee" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "en-GB", Value = "Mayhaps" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de", Value = "Vielleicht" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "", Language = "de-CH", Value = "Villicht" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "", Value = "Maybee (Industry)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "en-GB", Value = "Mayhaps (Industry)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "de", Value = "Vielleicht (Branche)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "", Industry = "IT", Language = "de-CH", Value = "Villicht (Branche)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "", Value = "Maybee (Customer)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "en-GB", Value = "Mayhaps (Customer)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "de", Value = "Vielleicht (Kunde)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "", Language = "de-CH", Value = "Villicht (Kunde)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "", Value = "Maybee (Industry + Customer)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "en-GB", Value = "Mayhaps (Industry + Customer)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "de", Value = "Vielleicht (Branche + Kunde)" });
            context.Add(new Resource { Id = "Maybe", ResourceSet = Constants.CommonTerms, BinFile = null, Textfile = null, Comment = null, Customer = "idee5", Industry = "IT", Language = "de-CH", Value = "Villicht (Branche + Kunde)" });
            // Save the data synchronously
            context.SaveChanges();
        }

        #region IDisposable Support
        private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    resourceUnitOfWork.Dispose();
                    context.Dispose();
                    _connection.Dispose();
                }

                _disposedValue = true;
            }
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
