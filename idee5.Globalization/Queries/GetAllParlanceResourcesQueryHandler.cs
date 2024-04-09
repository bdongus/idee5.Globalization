using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using NSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static idee5.Globalization.Specifications;

namespace idee5.Globalization.Queries;

/// <summary>
/// Returns all resources of a parlance. And only that parlance. With or without customer and industry parlance.
/// </summary>
public class GetAllParlanceResourcesQueryHandler : IQueryHandlerAsync<GetAllParlanceResourcesQuery, IEnumerable<Resource>> {
    #region Private Fields

    private readonly IResourceQueryRepository _repository;

    #endregion Private Fields

    #region Public Constructors

    public GetAllParlanceResourcesQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Execute the query to find all resources filtered by an industry and/or customer or limited to local
    /// or global resources. Ordered by resource set, language and resource id.
    /// </summary>
    /// <param name="query">The query filters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>Token for cancalling the operation.</param>
    /// <returns>The resource set as dictionary or </c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<IEnumerable<Resource>> HandleAsync(GetAllParlanceResourcesQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        // read industry parlance or just the neutral parlance
        ASpec<Resource> industryClause = String.IsNullOrEmpty(query.IndustryId) ? IndustryNeutral : IndustryParlance(query.IndustryId);
        // read custome parlance or just the neutral parlance
        ASpec<Resource> customerClause = String.IsNullOrEmpty(query.CustomerId) ? CustomerNeutral : CustomerParlance(query.CustomerId);
        ASpec<Resource> whereClause = industryClause & customerClause;
        if (query.LocalResources != null)
            whereClause &= query.LocalResources == true ? IsLocalResources : !IsLocalResources;
        List<Resource> resList = await _repository.GetAsync(whereClause, cancellationToken).ConfigureAwait(false);
        // TODO: Let the database do the ordering and grouping
        return resList.OrderBy(r => r.ResourceSet).ThenBy(r => r.Language).ThenBy(r => r.Id).ThenByDescending(r => r.Industry).ThenByDescending(r => r.Customer)
            .GroupBy(r => new { r.ResourceSet, r.Language, r.Id, r.BinFile, r.Comment, r.Textfile })
            .Select(g => g.First());
    }

    #endregion Public Methods
}