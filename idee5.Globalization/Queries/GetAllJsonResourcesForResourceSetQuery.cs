using idee5.Common;

namespace idee5.Globalization.Queries;
/// <summary>
/// Query parameter class to search for resources.
/// </summary>
/// <param name="ResourceSet"> Requested resource set </param>
public record GetAllJsonResourcesForResourceSetQuery(string ResourceSet) : IQuery<string>;
