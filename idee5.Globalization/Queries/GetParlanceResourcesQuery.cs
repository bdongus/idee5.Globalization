using idee5.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Queries;
/// <summary>
/// Query parameters for retieval of a parlance resource set
/// </summary>
/// <param name="ResourceSet"> Id of the resource set </param>
/// <param name="LanguageId"> An IETF languageId tag specifying the resource sets languageId </param>
/// <param name="CustomerId"> Id of the customer parlance </param>
/// <param name="IndustryId"> Id of the industry parlance </param>
public record GetParlanceResourcesQuery([property: Required] string ResourceSet, string LanguageId, string? CustomerId, string? IndustryId) : IQuery<Dictionary<string, object>>;
