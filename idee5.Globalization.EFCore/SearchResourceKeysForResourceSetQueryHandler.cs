using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Queries;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.EFCore;

/// <summary>
/// The search resource keys for resource set query handler.
/// </summary>
public class SearchResourceKeysForResourceSetQueryHandler : IQueryHandlerAsync<SearchResourceKeysForResourceSetQuery, IList<ResourceKey>> {
    #region Private Fields

    private readonly GlobalizationDbContext _context;

    #endregion Private Fields

    #region Public Constructors

    public SearchResourceKeysForResourceSetQueryHandler(GlobalizationDbContext context) => _context = context;

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Query the resource ids in a resource set. Including all parlances.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<IList<ResourceKey>> HandleAsync(SearchResourceKeysForResourceSetQuery query, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(query);
        return await _context.Resources.Where(Specifications.ContainsInResourceSet(query.ResourceSet, query.SearchValue)).Select(r => new ResourceKey() {
            // just casting results in all records being read
            ResourceSet = r.ResourceSet,
            Id          = r.Id,
            Industry    = r.Industry,
            Customer    = r.Customer
        }).Distinct().ToListAsync(cancellationToken);
    }
    #endregion Public Methods
}
