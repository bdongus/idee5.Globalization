namespace idee5.Globalization.Queries;

/// <summary>
/// Query parameters for retieval of a parlance resource set.
/// Resources in fallback languages are read too.
/// </summary>
/// <param name="ResourceSet"><inheritdoc/></param>
/// <param name="LanguageId"><inheritdoc/></param>
/// <param name="CustomerId"><inheritdoc/></param>
/// <param name="IndustryId"><inheritdoc/></param>
public record GetParlanceResourcesWithFallbackQuery(string ResourceSet, string LanguageId, string? CustomerId, string? IndustryId) : GetParlanceResourcesQuery(ResourceSet, LanguageId, CustomerId, IndustryId);
