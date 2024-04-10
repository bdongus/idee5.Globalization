using idee5.Globalization.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace idee5.Globalization.Commands;
/// <summary>
/// The update resource key command
/// </summary>
public record UpdateResourceKeyCommand : ResourceKey {
    public UpdateResourceKeyCommand(ResourceKey original, ImmutableList<Translation> translations) : base(original) {
        Translations = translations ?? throw new ArgumentNullException(nameof(translations));
    }

    /// <summary>
    /// Translations of the <see cref="ResourceKey">.
    /// The dictionary key is the <see cref="Resource.Language"/>, the value is the <see cref="Resource.Value"/>
    /// </summary>
    public ImmutableList<Translation> Translations { get; set; }
}
