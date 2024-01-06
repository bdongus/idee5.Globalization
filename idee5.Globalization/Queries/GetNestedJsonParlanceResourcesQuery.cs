using idee5.Common;

namespace idee5.Globalization.Queries;

/// <summary>
/// Query parameter class to search for resources.
/// </summary>
/// <param name="ResourceSet"><inheritdoc/></param>
/// <param name="LanguageId"><inheritdoc/></param>
/// <param name="CustomerId"><inheritdoc/></param>
/// <param name="IndustryId"><inheritdoc/></param>
public record GetNestedJsonParlanceResourcesQuery(string ResourceSet, string LanguageId, string? CustomerId, string? IndustryId)
    : GetParlanceResourcesQuery(ResourceSet, LanguageId, CustomerId, IndustryId), IQuery<string>;
