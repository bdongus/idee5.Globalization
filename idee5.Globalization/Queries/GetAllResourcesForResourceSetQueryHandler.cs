using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;

/// <summary>
/// Handler for reading a complete resource set. Including all parlances.
/// </summary>
public class GetAllResourcesForResourceSetQueryHandler : IQueryHandlerAsync<GetAllResourcesForResourceSetQuery, IList<Resource>> {
    #region Private Fields

    private readonly IResourceQueryRepository _repository;

    #endregion Private Fields

    #region Public Constructors

    public GetAllResourcesForResourceSetQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Query a complete resource set. Including all parlances.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<IList<Resource>> HandleAsync(GetAllResourcesForResourceSetQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));

        return await _repository.GetAsync(r => r.ResourceSet == query.ResourceSet, cancellationToken).ConfigureAwait(false);
    }

    #endregion Public Methods
}