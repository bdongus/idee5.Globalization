using idee5.Common;
using idee5.Common.Data;
using idee5.Globalization.EFCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace idee5.Globalization.Web
{
    /// <inheritdoc />
    public class GlobalizationSQLiteContext : GlobalizationDbContext
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        /// <inheritdoc />
        public GlobalizationSQLiteContext(DbContextOptions<GlobalizationSQLiteContext> options, IConnectionStringProvider connectionStringProvider) : base(options) {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        /// <summary>
        /// Constructor to allow derived contexts to be resolved by Microsoft DI.
        /// </summary>
        /// <remarks>See this github issue: https://github.com/aspnet/EntityFrameworkCore/issues/7533#issuecomment-353669263 </remarks>
        /// <param name="options"></param>
        protected GlobalizationSQLiteContext(DbContextOptions options, IConnectionStringProvider connectionStringProvider) : base(options) {
            _connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // work around for SQLite provider not resolving the DataDirectory| placeholder, fixed in 3.0.0-preview2
            optionsBuilder.UseSqlite(_connectionStringProvider.GetConnectionString("idee5.Resources.db3"));
        }
    }
}