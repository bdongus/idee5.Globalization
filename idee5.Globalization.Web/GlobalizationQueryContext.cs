using idee5.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace idee5.Globalization.Web {
    /// <inheritdoc />
    public class GlobalizationQueryContext : GlobalizationSQLiteContext {
        /// <inheritdoc />
        public GlobalizationQueryContext(DbContextOptions<GlobalizationQueryContext> options, IConnectionStringProvider connectionStringProvider) : base(options, connectionStringProvider) {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            System.ArgumentNullException.ThrowIfNull(optionsBuilder);

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}