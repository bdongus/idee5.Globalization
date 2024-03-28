using idee5.Common;
using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Queries;

/// <summary>
/// Search keys with a value in a resource set query.
/// </summary>
/// <param name="ResourceSet">Resource set to search in</param>
/// <param name="SearchValue">Value to search for. <c>NULL</c> means get all keys.</param>
public record SearchResourceKeysForResourceSetQuery(string ResourceSet, string? SearchValue = null) : IQuery<IList<ResourceKey>>;
