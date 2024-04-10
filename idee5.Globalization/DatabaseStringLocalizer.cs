using idee5.Globalization.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace idee5.Globalization;

/// <summary>
/// Base class for database string localizers
/// </summary>
public abstract class DatabaseStringLocalizer : IStringLocalizer {
    private readonly DatabaseResourceManager _resourceManager;
    protected readonly string? _industry;
    protected readonly string? _customer;

    protected DatabaseStringLocalizer(DatabaseResourceManager resourceManager, IOptions<LocalizationParlanceOptions> options) {
        _resourceManager = resourceManager;
        _industry = options.Value.Industry;
        _customer = options.Value.Customer;
    }
    /// <inheritdoc />
    public LocalizedString this[string name] {
        get {
            if (name == null) throw new ArgumentNullException(name);

            var value = _resourceManager.GetString(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceManager.BaseName);
        }
    }

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] {
        get {
            if (name == null) throw new ArgumentNullException(name);

            var format = _resourceManager.GetString(name);
            var value = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _resourceManager.BaseName);
        }
    }

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
        List<string> resourceNames = GetResourceNames(_resourceManager.BaseName, CultureHelper.GetCurrentCulture(), includeParentCultures);
        foreach (var name in resourceNames ?? Enumerable.Empty<string>()) {
            var value = _resourceManager.GetString(name, CultureInfo.CurrentUICulture);
            yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceManager.BaseName);
        }
    }

    /// <summary>
    /// Get all resource names with in a resource set
    /// </summary>
    /// <param name="resourceSet">The resource set</param>
    /// <param name="languageId">Language tag</param>
    /// <param name="includeParentCultures">Include the fallback language</param>
    /// <returns></returns>
    protected abstract List<string> GetResourceNames(string resourceSet, string languageId, bool includeParentCultures);
}
