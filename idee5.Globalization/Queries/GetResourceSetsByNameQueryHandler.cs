using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using idee5.Common;
using idee5.Globalization.Repositories;

namespace idee5.Globalization.Queries;
/// <summary>
/// The get resource sets by name query handler
/// </summary>
public class GetResourceSetsByNameQueryHandler : IQueryHandlerAsync<GetResourceSetsByNameQuery, IList<string>> {
    private readonly IResourceQueryRepository _repository;

    public GetResourceSetsByNameQueryHandler(IResourceQueryRepository repository) { _repository = repository; }

    /// <summary>
    /// Gets the resource sets by their name.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token for operation cancelling.</param>
    /// <returns>Returns the list of resource set names containing the query parameter.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<IList<string>> HandleAsync(GetResourceSetsByNameQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        List<string> result = await _repository.SearchResourceSetsAsync(query.Name, cancellationToken).ConfigureAwait(false);
        return result ?? [];
    }
}