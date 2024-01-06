using idee5.Common;
using idee5.Globalization.Models;
using System.Collections.Generic;

namespace idee5.Globalization.Queries;
/// <summary>
/// Query parameters for reading a complete resource set. Including all parlances.
/// </summary>
/// <param name="ResourceSet"> Requested resource set </param>
public record GetAllResourcesForResourceSetQuery(string ResourceSet) : IQuery<IList<Resource>>;
