using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Commands;
/// <summary>
/// The update resource key command
/// </summary>
public record CreateOrUpdateResourceKeyCommand : ResourceTranslations {
    public CreateOrUpdateResourceKeyCommand(ResourceKey original, IList<Translation> translations) : base(original, translations) {
    }

    public CreateOrUpdateResourceKeyCommand(string resourceSet, string id, string? industry, string? customer, IList<Translation> translations) : base(resourceSet, id, industry, customer, translations) {
    }
}
