using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.EFCore {
    /// <summary>
    /// EF Core implementation of the <see cref="AResourceRepository"/>.
    /// </summary>
    public class ResourceRepository : AResourceRepository {
        #region Private Fields

        private readonly GlobalizationDbContext _context;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize an EF Core <see cref="ResourceRepository"/>.
        /// </summary>
        /// <param name="context">EF Core db context.</param>
        public ResourceRepository(GlobalizationDbContext context) {
            _context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc />
        public override void Add(Resource item) {
            ArgumentNullException.ThrowIfNull(item);

            _context.Add(item);
        }

        /// <inheritdoc />
        public override void Remove(Resource item) {
            ArgumentNullException.ThrowIfNull(item);
            Resource itemToRemove = GetEntity(item)?.Entity ?? item;
            _context.Remove(itemToRemove);
        }

        /// <summary>
        /// Retrieve the change tracking entry by its id fields.
        /// </summary>
        /// <param name="item">The item to look up</param>
        /// <returns>NULL if the item isn't tracked yet.</returns>
        private EntityEntry<Resource>? GetEntity(Resource item) => _context
                            .ChangeTracker
                            .Entries<Resource>()
                            .SingleOrDefault(r => r.Entity.Language == item.Language && r.Entity.Id == item.Id && r.Entity.ResourceSet == item.ResourceSet && r.Entity.Industry == item.Industry && r.Entity.Customer == item.Customer);

        /// <inheritdoc />
        public override async Task RemoveAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            ArgumentNullException.ThrowIfNull(predicate);

            Resource[] toBeDeleted = await _context.Resources.Where(predicate).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            if (toBeDeleted?.Length > 0) {
                _context.RemoveRange(toBeDeleted);
            }
        }

        /// <inheritdoc />
        public override void Update(Resource item) {
            ArgumentNullException.ThrowIfNull(item);
            EntityEntry<Resource>? entry = GetEntity(item);
            if (entry == null) {
                // the item isn't tracked yet, start tracking it
                _context.Update(item);
            } else if (entry.State != EntityState.Deleted) {
                // the item is tracked and not deleted, update the non-key properties
                entry.CurrentValues.SetValues(item);
            }
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(Expression<Func<Resource, bool>> predicate, Action<Resource> action, CancellationToken cancellationToken) {
            Resource[] items = await _context.Resources.Where(predicate).ToArrayAsync(cancellationToken).ConfigureAwait(false);
            if (items?.Length > 0) {
                items.ForEach(action);
            }
        }

        /// <inheritdoc/>
        public override Task<List<string>> SearchResourceSetsAsync(string contains, CancellationToken cancellationToken = default) {
            return _context.Resources.Where(r => r.ResourceSet.Contains(contains))
                .Select(r => r.ResourceSet).Distinct().ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<List<Resource>> GetAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<bool> ExistsAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<int> CountAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<Resource?> GetSingleAsync(Expression<Func<Resource, bool>> predicate, CancellationToken cancellationToken = default) {
            return _context.Resources.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default) {
            return _context.Resources.ToListAsync(cancellationToken);
        }

        #endregion Public Methods
    }
}
