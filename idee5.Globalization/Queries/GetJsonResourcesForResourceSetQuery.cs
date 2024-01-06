using idee5.Common;

namespace idee5.Globalization.Queries;

/// <summary>
/// Query parameter class to get a specific resource set.
/// </summary>
public record GetJsonResourcesForResourceSetQuery : GetParlanceResourcesQuery, IQuery<string> {
    public GetJsonResourcesForResourceSetQuery(string ResourceSet, string LanguageId, string? CustomerId, string? IndustryId) : base(ResourceSet, LanguageId, CustomerId, IndustryId) {
    }
}