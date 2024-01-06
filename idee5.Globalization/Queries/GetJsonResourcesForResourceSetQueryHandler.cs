using idee5.Common;
using idee5.Globalization.Repositories;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;

public class GetJsonResourcesForResourceSetQueryHandler : IQueryHandlerAsync<GetJsonResourcesForResourceSetQuery, string> {
    private readonly IResourceQueryRepository _repository;

    public GetJsonResourcesForResourceSetQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The JSON representation of the resource set dictionary.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<string> HandleAsync(GetJsonResourcesForResourceSetQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new System.ArgumentNullException(nameof(query));
        // we rely on this dependency, thus it is not injected
        var qh = new GetParlanceResourcesWithFallbackQueryHandler(_repository);
        IDictionary<string, object> result = await qh.HandleAsync(new GetParlanceResourcesWithFallbackQuery(query.ResourceSet, query.LanguageId, query.CustomerId, query.IndustryId), cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        return JsonSerializer.Serialize(result);
    }
}