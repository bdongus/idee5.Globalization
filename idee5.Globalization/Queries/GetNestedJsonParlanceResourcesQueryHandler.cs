using idee5.Common;
using idee5.Globalization.Repositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;

/// <summary>
/// Read a resource set with the given parlance and retrn it as nested JSON.
/// This allows javascript to access the resource with "dot" syntax. E.g. Level1.Level2.Property
/// </summary>
public class GetNestedJsonParlanceResourcesQueryHandler : IQueryHandlerAsync<GetNestedJsonParlanceResourcesQuery, string> {
    private readonly IResourceQueryRepository _repository;

    public GetNestedJsonParlanceResourcesQueryHandler(IResourceQueryRepository repository) {
        _repository = repository;
    }

    /// <summary>
    /// Handles the specified query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The JSON represenation of the resource set.</returns>
    public async Task<string> HandleAsync(GetNestedJsonParlanceResourcesQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        // the client needs the fallbacks included
        var qh = new GetParlanceResourcesWithFallbackQueryHandler(_repository);
        IDictionary<string, object> items = await qh.HandleAsync(new GetParlanceResourcesWithFallbackQuery(query.ResourceSet, query.LanguageId, query.CustomerId, query.IndustryId)).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        // create the nested hierachy
        var expand = new ExpandoObject();
        foreach (KeyValuePair<string, object> item in items) {
            string[] levels = item.Key.Split('.');
            AddChildren(expand, levels, item.Value);
        }
        cancellationToken.ThrowIfCancellationRequested();
        return JsonSerializer.Serialize(expand);
    }

    /// <summary>
    /// Recursively add child nodes to the parent.
    /// </summary>
    /// <param name="parentNode">Parent node the child is added to.</param>
    /// <param name="levels">String array representing the depth/hierarchy. </param>
    /// <param name="value">Poperty value to be set at the bottom level.</param>
    /// <exception cref="ArgumentNullException"><paramref name="parentNode"/> or <paramref name="levels"/> is <c>null</c>.</exception>
    protected void AddChildren(ExpandoObject parentNode, string[] levels, object value) {
        if (parentNode == null)
            throw new ArgumentNullException(nameof(parentNode));

        if (levels == null)
            throw new ArgumentNullException(nameof(levels));

        if (levels.Length > 1) {
            // if there is another level, dive deeper
            var p = parentNode.GetPropertyByName(levels[0]) ?? new ExpandoObject();
            parentNode.AddProperty(levels[0], p);
            AddChildren(p as ExpandoObject, levels.Skip(1).ToArray(), value);
        } else {
            // the bottom is reached, set the property value
            parentNode.AddProperty(levels[0], value);
        }
    }
}