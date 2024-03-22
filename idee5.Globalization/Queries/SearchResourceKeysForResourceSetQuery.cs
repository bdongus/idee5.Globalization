using idee5.Common;
using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Queries;

/// <summary>
/// Search a value in a resource set query.
/// </summary>
/// <param name="ResourceSet">Resource set to search in</param>
/// <param name="SearchValue">Value to search for</param>
public record SearchResourceKeysForResourceSetQuery(string ResourceSet, string SearchValue) : IQuery<IList<ResourceKey>>;
