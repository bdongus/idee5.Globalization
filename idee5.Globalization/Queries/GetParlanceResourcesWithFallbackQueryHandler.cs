using idee5.Common;
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
/// Returns a specific set of resources for a given culture and 'resource set'.
/// Used for sceanrios here no <see cref="System.Resources.ResourceManager"/> is handling fallbacks.
/// E.g. Returning a resource set to a client.
/// </summary>
public class GetParlanceResourcesWithFallbackQueryHandler : IQueryHandlerAsync<GetParlanceResourcesWithFallbackQuery, Dictionary<string, object>> {
    #region Private Fields

    private readonly IResourceQueryRepository _repository;

    #endregion Private Fields

    #region Public Constructors

    public GetParlanceResourcesWithFallbackQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Execute the query to find a specific set of resources for a given culture and 'resource set'
    /// </summary>
    /// <param name="query">The query filters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The resource set as dictionary.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public async Task<Dictionary<string, object>> HandleAsync(GetParlanceResourcesWithFallbackQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var resources = new Dictionary<string, object>();
        ASpec<Models.Resource> languageSpec = query.LanguageId.HasValue() ? OfLanguageOrFallback(query.LanguageId) : NeutralLanguage;
        ASpec<Models.Resource> parlanceSpec = InResourceSet(query.ResourceSet) & languageSpec & CustomerParlance(query.CustomerId) & IndustryParlance(query.IndustryId);

        // get the resources
        IEnumerable<Models.Resource> r1 = await _repository.GetAsync(parlanceSpec, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        // Extract the relevant resource and set the proper resource value.
        resources = r1
            .OrderBy(r => r.Id)
            .ThenByDescending(r => r.Industry)
            .ThenByDescending(r => r.Customer)
            .ThenByDescending(r => r.Language)
            .GroupBy(r => r.Id)
            .ToDictionary(g => g.Key, g => (object)
                (g.First().Is(TextfileResource) ? (g.First().Textfile ?? "") :
                g.First().Is(BinaryFileResource) ? (g.First().BinFile ?? []) :
                g.First().Value)
             );
        cancellationToken.ThrowIfCancellationRequested();
        return resources;
    }

    #endregion Public Methods
}