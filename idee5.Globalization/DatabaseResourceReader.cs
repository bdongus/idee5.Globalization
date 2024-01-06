using idee5.Globalization.Queries;
using idee5.Globalization.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization;
/// <summary>
/// This class uses the resource repository to retrieve a list of resources for a given resource
/// set and language. Using an IEnumerable list, it returns the same structure as resources read
/// from ResX files. If provided, it uses the industry and customer to retrieve specialized
/// resource sets.
/// </summary>
public class DatabaseResourceReader : IEnumerable, IDisposable, IResourceReader {
    private readonly GetParlanceResourcesQuery _getParlanceResourcesQuery;
    private readonly GetParlanceResourcesQueryHandler _getParlanceResourcesQueryHandler;
    /// <summary>
    /// Item cache. To reduce database/disk access.
    /// </summary>
    private Dictionary<string, object>? _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResourceReader"/> class.
    /// </summary>
    /// <param name="repository">The database context.</param>
    /// <param name="resourceSet">The resource set.</param>
    /// <param name="language">The language.</param>
    /// <param name="industry">The industry.</param>
    /// <param name="customer">The customer.</param>
    public DatabaseResourceReader(IResourceRepository repository, string resourceSet, string language, string? industry, string? customer) {
        // store the query parameters
        _getParlanceResourcesQuery = new GetParlanceResourcesQuery(resourceSet, language, customer, industry);

        _getParlanceResourcesQueryHandler = new GetParlanceResourcesQueryHandler(repository);
    }

    /// <summary>Returns an enumerator that iterates through a resource collection.</summary>
    /// <returns>An <see cref="IEnumerator"></see> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close() { Dispose(); }

    /// <summary>
    /// <para>
    /// Query a specific resource set and language and returs a Hashtable (as IEnumerable) to use
    /// as a ResourceSet.
    /// </para>
    /// <para>
    /// Resources are read once and then cached. A ResourceReader instance is bound to a resource
    /// set and language combination. Reloading is only needed when explicitly clearing the
    /// reader/resource set.
    /// </para>
    /// </summary>
    /// <returns></returns>
    public IDictionaryEnumerator GetEnumerator() {
        if (_items == null) {
            Task<Dictionary<string, object>> task = _getParlanceResourcesQueryHandler.HandleAsync(_getParlanceResourcesQuery, CancellationToken.None);
            // handle the async method sychronously
            _items = task.GetAwaiter().GetResult();
        }
        return _items.GetEnumerator();
    }

    #region IDisposable Support
    private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                _items = null; // clear the chache
            }
            _disposedValue = true;
        }
    }

    public void Dispose() {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
