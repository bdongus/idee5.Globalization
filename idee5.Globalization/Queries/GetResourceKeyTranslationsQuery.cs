using idee5.Common;
using idee5.Globalization.Models;

namespace idee5.Globalization.Queries;

/// <summary>
/// The get resource key translations query parameters
/// </summary>
public record GetResourceKeyTranslationsQuery() : ResourceKey, IQuery<ResourceTranslations>;
