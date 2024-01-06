using System.Collections.Generic;
using idee5.Common;

namespace idee5.Globalization.Queries;

/// <summary>
/// Query parameter class to search for resource sets
/// </summary>
/// <param name="Name">The text contained in the resource sets name/id </param>
public record GetResourceSetsByNameQuery(string Name) : IQuery<IList<string>>;
