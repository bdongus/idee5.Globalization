using idee5.Common;
using idee5.Globalization.Models;

using System.Collections.Generic;

namespace idee5.Globalization.Queries;
/// <summary>
/// The get the resource keys for a resource set query.
/// </summary>
public record GetResourceKeysForResourceSetQuery(string ResourceSet) : IQuery<IList<ResourceKey>>;
