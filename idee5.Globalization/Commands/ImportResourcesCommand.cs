using idee5.Globalization.Models;
using System.Collections.Generic;

namespace idee5.Globalization.Commands;

/// <summary>
/// Import multiple resources command
/// </summary>
/// <param name="Resources"> List of <see cref="Resource"/>s to be imported. </param>
public record ImportResourcesCommand(IEnumerable<Resource> Resources);
