using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;

/// <summary>
/// The get resource key translations query handler
/// </summary>
public class GetResourceKeyTranslationsQueryHandler : IQueryHandlerAsync<GetResourceKeyTranslationsQuery, ResourceTranslations> {
    readonly IResourceQueryRepository _repository;

    public GetResourceKeyTranslationsQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<ResourceTranslations> HandleAsync(GetResourceKeyTranslationsQuery query, CancellationToken cancellationToken = default) {
        if (query is null) {
            throw new ArgumentNullException(nameof(query));
        }
        List<Resource> res = await _repository.GetAsync(Specifications.OfResourceKey(query)).ConfigureAwait(false);
        Translation[] t = res.Select(r => new Translation(r.Language ?? "", r.Value, r.Comment)).ToArray();
        return new(query, t);
    }
}
