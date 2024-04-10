using idee5.Globalization.Models;
using idee5.Globalization.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.EFCore {
    /// <inheritdoc />
    public class ResourceQueryRepository : IResourceQueryRepository {
        /// <summary>
        /// EF core context.
        /// </summary>
        protected readonly GlobalizationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AResourceRepository"/> class.
        /// </summary>
        /// <param name="context">The context. Should be a non tracking for performance improvement.</param>
        public ResourceQueryRepository(GlobalizationDbContext context) {
            _context = context;
        }

        /// <inheritdoc/>
        public Task<int> CountAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking().CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking().AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking().ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<Resource>> GetAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<Resource?> GetSingleAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<string>> SearchResourceSetsAsync(string contains, CancellationToken cancellationToken = default) {
            return _context.Resources.AsNoTracking()
                .Where(r => r.ResourceSet.Contains(contains))
                .Select(r => r.ResourceSet).Distinct().ToListAsync(cancellationToken);
        }
    }
}