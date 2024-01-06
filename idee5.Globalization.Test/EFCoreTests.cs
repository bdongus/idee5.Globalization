using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace idee5.Globalization.Test
{
    [TestClass]
    public class EFCoreTests
    {
        private TestDbContext _context;

        public class TestEntity
        {
            public string Id1 { get; set; }
            public string Id2 { get; set; }
            public string Value { get; set; }
        }
        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
            { }

            public DbSet<TestEntity> Testentities { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestEntity>().ToTable("tst");
                modelBuilder.Entity<TestEntity>().HasKey(e => new { e.Id1, e.Id2 });
                modelBuilder.Entity<TestEntity>().Property(r => r.Value).HasMaxLength(255).HasColumnName("val");
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<TestEntity>, TResult> func, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => func(_context.Testentities.AsNoTracking()), cancellationToken);
        }

        [TestMethod]
        public async Task TestMethod1Async()
        {
            // Arrange

            var contextOptions = new DbContextOptionsBuilder<TestDbContext>();

            contextOptions.UseSqlite("data source=test.db3");
            contextOptions.EnableSensitiveDataLogging();
            _context = new TestDbContext(contextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Add the test data
            _context.Add(new TestEntity { Id1 = "To", Id2="Be", Value = "Or not" });
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var toImport = new TestEntity[] {
                new() { Id1 = "To", Id2="Be", Value = "Or not to be" },
                new() { Id1 = "Make", Id2="my", Value = "day" },
            };

            // Act
            foreach (var item in toImport) {
                bool isUpdate = await GetAsync(q => q.Any(t => t.Id1 == item.Id1 && t.Id2 == item.Id2)).ConfigureAwait(false);
                if (isUpdate) {
                    var entry = _context.ChangeTracker.Entries<TestEntity>().SingleOrDefault(e => e.Entity.Id1 == item.Id1 && e.Entity.Id2 == item.Id2);
                    if (entry == null)
                        // This triggers ThrowIdentityConflict in EF Core ?!
                        _context.Update(item);
                    else if (entry.State != EntityState.Deleted) {
                        // the item is tracked and not deleted, update the non-key properties
                        entry.CurrentValues.SetValues(item);
                    }
                } else {
                    _context.Add(item);
                }
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Assert
            TestEntity actual = await _context.Testentities.SingleAsync(t => t.Id1 == "To" && t.Id2 == "Be").ConfigureAwait(false);
            Assert.AreEqual("Or not to be", actual.Value);
        }

        [TestMethod]
        public async Task TestMethod2Async()
        {
            // Arrange

            var contextOptions = new DbContextOptionsBuilder<TestDbContext>();

            contextOptions.UseSqlite("data source=test2.db3");
            contextOptions.EnableSensitiveDataLogging();
            _context = new TestDbContext(contextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Add the test data
            _context.Add(new TestEntity { Id1 = "To", Id2="Be", Value = "Or not" });
            await _context.SaveChangesAsync().ConfigureAwait(false);
            var toImport = new TestEntity[] {
                new() { Id1 = "To", Id2="Be", Value = "Or not to be" },
                new() { Id1 = "Make", Id2="my", Value = "day" },
            };

            // Act
            foreach (var item in toImport) {
                bool isUpdate = await GetAsync(q => q.Any(t => t.Id1 == item.Id1 && t.Id2 == item.Id2)).ConfigureAwait(false);
                if (isUpdate) {
                    var itemToUpdate = await _context.Testentities.SingleAsync(t => t.Id1 == item.Id1 && t.Id2 == item.Id2).ConfigureAwait(false);
                    itemToUpdate.Value = item.Value;
                }
                else
                    _context.Add(item);
            }
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Assert
            TestEntity actual = await _context.Testentities.SingleAsync(t => t.Id1 == "To" && t.Id2 == "Be").ConfigureAwait(false);
            Assert.AreEqual("Or not to be", actual.Value);
        }

        [TestMethod]
        public async Task CanRemoveEntity()
        {
            // Arrange

            var contextOptions = new DbContextOptionsBuilder<TestDbContext>();

            contextOptions.UseSqlite("data source=test3.db3");
            contextOptions.EnableSensitiveDataLogging();
            _context = new TestDbContext(contextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Add the test data
            _context.Add(new TestEntity { Id1 = "To", Id2="Be", Value = "Or not" });
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Act
            var itemToRemove = await _context.Testentities.SingleAsync(t => t.Id1 == "To" && t.Id2 == "Be").ConfigureAwait(false);
            _context.Testentities.Remove(itemToRemove);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Assert
            TestEntity actual = await _context.Testentities.SingleOrDefaultAsync(t => t.Id1 == "To" && t.Id2 == "Be").ConfigureAwait(false);
            Assert.AreEqual(null, actual);
        }
    }
}
