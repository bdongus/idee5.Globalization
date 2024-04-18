using idee5.Globalization.Models;

using System;
using System.Collections.Immutable;

namespace idee5.Globalization.Commands;
/// <summary>
/// The update resource key command
/// </summary>
public record CreateOrUpdateResourceKeyCommand : ResourceKey {
    public CreateOrUpdateResourceKeyCommand(ResourceKey original, ImmutableList<Translation> translations) : base(original) {
        Translations = translations ?? throw new ArgumentNullException(nameof(translations));
    }

    /// <summary>
    /// Translations of the <see cref="ResourceKey">.
    /// </summary>
    public ImmutableList<Translation> Translations { get; set; }
}
