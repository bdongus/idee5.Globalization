using System;
using System.Collections.Generic;

namespace idee5.Globalization.Models;
/// <summary>
/// The resource translations
/// </summary>
public record ResourceTranslations : ResourceKey {
    public ResourceTranslations() {
        Translations = [];
    }
    public ResourceTranslations(string resourceSet, string id, string? industry, string? customer, IList<Translation> translations) {
        ResourceSet = resourceSet;
        Id = id;
        Industry = industry;
        Customer = customer;
        Translations = translations;
    }

    public ResourceTranslations(ResourceKey original, IList<Translation> translations) : base(original) {
        Translations = translations ?? throw new ArgumentNullException(nameof(translations));
    }

    /// <summary>
    /// Translations of the <see cref="ResourceKey">.
    /// </summary>
    public IList<Translation> Translations { get; set; }
}
