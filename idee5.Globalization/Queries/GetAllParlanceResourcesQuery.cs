using idee5.Common;
using idee5.Globalization.Models;
using System.Collections.Generic;

namespace idee5.Globalization.Queries;
/// <summary>
/// Query parameters for retrieval of all resources of a parlance. And only that parlance
/// </summary>
/// <param name="LocalResources"> NULL = Retrieve ALL resources. True = Retrieve local resources. False = Retrieve global resources. </param>
/// <param name="CustomerId"> Id of the customer parlance. NULL = only read common parlance, otherwise include the customer parlance. </param>
/// <param name="IndustryId"> Id of the industry parlance. NULL = only read common parlance, otherwise include the industry parlance. </param>
public record GetAllParlanceResourcesQuery(bool? LocalResources, string? CustomerId, string? IndustryId) : IQuery<IEnumerable<Resource>>;
