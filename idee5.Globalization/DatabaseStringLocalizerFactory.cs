using idee5.Globalization.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace idee5.Globalization;

/// <summary>
/// Base class for database string localizer factories
/// </summary>
public abstract class DatabaseStringLocalizerFactory : IStringLocalizerFactory {
    protected readonly IOptions<LocalizationParlanceOptions> _options;
    private readonly ConcurrentDictionary<string, DatabaseStringLocalizer> _localizerCache = new();
    protected DatabaseStringLocalizerFactory(IOptions<LocalizationParlanceOptions> options) {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(Type resourceSource) {
        if (resourceSource is null) throw new ArgumentNullException(nameof(resourceSource));

        TypeInfo typeInfo = resourceSource.GetTypeInfo();
        if (string.IsNullOrEmpty(typeInfo.FullName)) {
            throw new ArgumentException($"Type must have type name {typeInfo}");
        }
        string resourceSet = typeInfo.FullName;
        return GetLocalizer(resourceSet);
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(string baseName, string location) {
        if (baseName is null) throw new ArgumentNullException(nameof(baseName));
        if (location is null) throw new ArgumentNullException(nameof(location));

        // remove the location part from the base name, if neccessary
        var prefix = location + ".";
        if (baseName.StartsWith(prefix, StringComparison.Ordinal)) {
            baseName = baseName.Substring(prefix.Length);
        }
        // if the location is an assembly name, try to load the assembly and get the root namespace
        try {
            var assemblyName = new AssemblyName(location);
            var assembly = Assembly.Load(assemblyName);
            var rootNamespace = assembly.GetName().Name;

            return GetLocalizer(rootNamespace + "." + baseName);
        }
        catch (Exception) {
            // if there is no assembly use the base name. This is an additional feature and not part of the original implementation
            // this is meant for imported resx files and not for embedded resources
            return GetLocalizer(baseName);
        }
    }
    private IStringLocalizer GetLocalizer(string resourceSet) {
        // Get without Add to prevent unnecessary lambda allocation
        if (!_localizerCache.TryGetValue(resourceSet, out DatabaseStringLocalizer localizer)) {
            localizer = CreateResourceManagerStringLocalizer(resourceSet);

            _localizerCache[resourceSet] = localizer;
        }

        return localizer;
    }

    /// <summary>
    /// Creates a <see cref="DatabaseStringLocalizer"/> for the given input
    /// </summary>
    /// <param name="resourceSet">Resource set the localizer is created for</param>
    protected abstract DatabaseStringLocalizer CreateResourceManagerStringLocalizer(string resourceSet);
}
