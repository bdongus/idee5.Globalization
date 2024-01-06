using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using static idee5.Globalization.Specifications;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace idee5.Globalization.Queries;
public class GetExactResourceQueryHandler : IQueryHandlerAsync<GetExactResourceQuery, Resource?> {
    #region Private Fields

    private readonly IResourceQueryRepository _resourceQueryRepository;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Create the query handler.
    /// </summary>
    /// <param name="resourceQueryRepository">The query repository used for the query.</param>
    public GetExactResourceQueryHandler(IResourceQueryRepository resourceQueryRepository) {
        _resourceQueryRepository = resourceQueryRepository;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Find exactly the specified resource. No hierachical search or other fancy logic.
    /// </summary>
    /// <param name="query">The resource to look for.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>Null</c> or the found <see cref="Resource"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <c>null</c>.</exception>
    public Task<Resource?> HandleAsync(GetExactResourceQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return _resourceQueryRepository.GetSingleAsync(ResourceId(query.Id)
            & InResourceSet(query.ResourceSet)
            & OfLanguage(query.Language)
            & CustomerParlance(query.Customer)
            & IndustryParlance(query.Industry), cancellationToken);
    }

    #endregion Public Methods
}
