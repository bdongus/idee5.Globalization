using idee5.Globalization.Models;

using Microsoft.EntityFrameworkCore;

namespace idee5.Globalization.EFCore {
    /// <summary>
    /// EF Core base context class. The database connection has to be defined in the repository.
    /// </summary>
    public class GlobalizationDbContext : DbContext {
        /// <summary>
        /// Default connection string name.
        /// </summary>
        public static readonly string connectionStringName = "idee5ResourcesConnection";

        /// <summary>
        /// Connection string to the resources storage.
        /// </summary>
        protected string? connectionString;

        /// <summary>
        /// Create a context with the given options.
        /// </summary>
        /// <param name="options"><see cref="DbContextOptions{GlobalizationDbContext}"/> created with a <see cref="DbContextOptionsBuilder{GlobalizationDbContext}"/>.</param>
        /// <example>
        /// var contextOptions = new DbContextOptionsBuilder&lt;GlobalizationDbContext&gt;();
        /// contextOptions.UseSqlite("data source=|DataDirectory|idee5.Resources.db3");
        /// context = new GlobalizationDbContext(contextOptions.Options);
        /// </example>
        public GlobalizationDbContext(DbContextOptions<GlobalizationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Constructor to allow derived contexts to be resolved by Microsoft DI.
        /// </summary>
        /// <remarks>See this github issue: https://github.com/aspnet/EntityFrameworkCore/issues/7533#issuecomment-353669263 </remarks>
        /// <param name="options"></param>
        protected GlobalizationDbContext(DbContextOptions options) : base(options) {

        }
        /// <summary>
        /// The <see cref="Resource"/>s in the database.
        /// </summary>
        public DbSet<Resource> Resources { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Resource>().ToTable("resrce");
            modelBuilder.Entity<Resource>().HasKey(r => new { r.ResourceSet, r.Language, r.Id, r.Industry, r.Customer });
            modelBuilder.Entity<Resource>().Property(r => r.ResourceSet).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Resource>().Property(r => r.Language).HasMaxLength(10).HasColumnName("lnguage").IsRequired();
            modelBuilder.Entity<Resource>().Property(r => r.Industry).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Resource>().Property(r => r.Customer).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Resource>().Property(r => r.Id).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Resource>().Property(r => r.Value).HasMaxLength(255).HasColumnName("val");
            modelBuilder.Entity<Resource>().Property(r => r.Comment).HasMaxLength(255).HasColumnName("commnt");
            modelBuilder.Entity<Resource>().Property(r => r.Textfile).HasMaxLength(255);
            modelBuilder.Entity<Resource>().Property(r => r.BinFile).HasMaxLength(255);
        }
    }
}
