using idee5.Common;
using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Queries;

/// <summary>
/// Search a value in all resource sets query.
/// </summary>
/// <param name="SearchValue">Value to search for</param>
public record SearchResourceKeysQuery(string SearchValue) : IQuery<IList<ResourceKey>>;
