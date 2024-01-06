using idee5.Globalization.Repositories;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace idee5.Globalization;
public class DatabaseResourceManager : ResourceManager {
    private readonly IResourceRepository _repository;
    private readonly string _resourceSet;
    private readonly string? _industry;
    private readonly string? _customer;
    private readonly Dictionary<string, ResourceSet> _internalResourceSets;

    /// <summary>
    /// Critical Section lock used for loading/adding resource sets
    /// </summary>
    private static readonly object _syncLock = new();

    public override string BaseName => _resourceSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResourceManager"/> class.
    /// </summary>
    /// <param name="repository">The db context.</param>
    /// <param name="resourceSet">The resource set.</param>
    /// <param name="industry">The industry.</param>
    /// <param name="customer">The customerId.</param>
    public DatabaseResourceManager(IResourceRepository repository, string resourceSet, string? industry, string? customer) {
        _customer = customer;
        _industry = industry;
        _resourceSet = resourceSet;
        _repository = repository;
        _internalResourceSets = [];
    }

    /// <summary>
    /// The methods of the resource manager that you can use to access resources (e.g. GetString
    /// and GetObject) invoke the method <see cref="ResourceManager.InternalGetResourceSet"/>.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <param name="createIfNotExists">The create if not exists.</param>
    /// <param name="tryParents">The try parents.</param>
    protected override ResourceSet? InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents) {
        ResourceSet? rs = null;
        if (culture != null) {
            if (_internalResourceSets.ContainsKey(culture.Name)) {
                rs = _internalResourceSets[culture.Name];
            } else {
                lock (_syncLock) {
                    // check if resource set was read while waiting
                    if (_internalResourceSets.ContainsKey(culture.Name)) {
                        rs = _internalResourceSets[culture.Name];
                    } else {
                        rs = new DatabaseResourceSet(_repository, _resourceSet, culture, _industry, _customer);
                        _internalResourceSets.Add(culture.Name, rs);
                    }
                }
            }
        }

        return rs;
    }

    /// <summary>
    /// Tells the resource manager to call the <see cref="ResourceSet.Close"/>
    /// method on all <see cref="ResourceSet"/> objects and release all
    /// resources. Needed to see database changes in the UI.
    /// </summary>
    public override void ReleaseAllResources() {
        base.ReleaseAllResources();
        _internalResourceSets?.Clear();
    }
}