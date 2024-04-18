using System;

namespace idee5.Globalization.Models;
/// <summary>
/// The resource translations
/// </summary>
public record ResourceTranslations : ResourceKey {
    public ResourceTranslations(ResourceKey original, Translation[] translations) : base(original) {
        Translations = translations ?? throw new ArgumentNullException(nameof(translations));
    }

    /// <summary>
    /// Translations of the <see cref="ResourceKey">.
    /// </summary>
    public Translation[] Translations { get; set; }
}
