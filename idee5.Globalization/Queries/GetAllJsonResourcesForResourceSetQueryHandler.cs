using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;
/// <summary>
/// The get all json resources for resource set.
/// </summary>
public class GetAllJsonResourcesForResourceSetQueryHandler : IQueryHandlerAsync<GetAllJsonResourcesForResourceSetQuery, string> {
    private readonly IResourceQueryRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllJsonResourcesForResourceSetQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The resource repository.</param>
    public GetAllJsonResourcesForResourceSetQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The JSON represenation of the resources.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<string> HandleAsync(GetAllJsonResourcesForResourceSetQuery query, CancellationToken cancellationToken = default) {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var qh = new GetAllResourcesForResourceSetQueryHandler(_repository);
        IList<Resource> result = await qh.HandleAsync(new GetAllResourcesForResourceSetQuery(query.ResourceSet), cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return JsonSerializer.Serialize(result);
    }
}