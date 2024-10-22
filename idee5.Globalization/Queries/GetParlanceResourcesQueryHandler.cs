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
/// This mimics the behaviour expected by a .NET <see cref="System.Resources.IResourceReader"/>
/// </summary>
public class GetParlanceResourcesQueryHandler : IQueryHandlerAsync<GetParlanceResourcesQuery, Dictionary<string, object>> {
    #region Private Fields

    private readonly IResourceQueryRepository _repository;

    #endregion Private Fields

    #region Public Constructors

    public GetParlanceResourcesQueryHandler(IResourceQueryRepository repository) {
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
    public async Task<Dictionary<string, object>> HandleAsync(GetParlanceResourcesQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        ASpec<Models.Resource> languageSpec = query.LanguageId.HasValue() ? OfLanguage(query.LanguageId) : NeutralLanguage;
        ASpec<Models.Resource> parlanceSpec = InResourceSet(query.ResourceSet) & languageSpec & CustomerParlance(query.CustomerId) & IndustryParlance(query.IndustryId);

        // get the resources
        IEnumerable<Models.Resource> reslist = await _repository.GetAsync(parlanceSpec, cancellationToken).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();
        // Extract the relevant resource and set the proper resource value.
        IEnumerable<IGrouping<string, Models.Resource>> r = reslist
            .OrderBy(r => r.Id)
            .ThenByDescending(r => r.Industry)
            .ThenByDescending(r => r.Customer)
            .GroupBy(r => r.Id);
        cancellationToken.ThrowIfCancellationRequested();
        Dictionary<string, object> resources = r.ToDictionary(g => g.Key, g =>(object)
            (g.First().Is(TextfileResource) ? (g.First().Textfile ?? "") :
            g.First().Is(BinaryFileResource) ? (g.First().BinFile ?? []) :
            g.First().Value)
        );
        cancellationToken.ThrowIfCancellationRequested();
        return resources;
    }

    #endregion Public Methods
}