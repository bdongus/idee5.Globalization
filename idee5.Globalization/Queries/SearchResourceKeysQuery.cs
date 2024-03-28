using idee5.Common;
using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Queries;

/// <summary>
/// Search a value in all resource sets query.
/// </summary>
/// <param name="SearchValue">Value to search for. <c>NULL</c> means get all keys.</param>
public record SearchResourceKeysQuery(string? SearchValue = null) : IQuery<IList<ResourceKey>>;
