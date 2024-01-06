using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Commands;

/// <summary>
/// Command to generate resources. Used for <see cref="System.Resources.ResourceWriter.Generate()"/>.
/// </summary>
/// <param name="ResourceList"> Dictionary of resources to be persisted </param>
/// <param name="ResourceSet"> Id of the resource set </param>
/// <param name="LanguageId"> An IETF languageId tag specifying the resource sets languageId </param>
/// <param name="DeleteAllResourcesFirst"> If true wipe the resource set before persiting it </param>
/// <param name="Industry"> Id of the industry parlance </param>
/// <param name="Customer"> Id of the customer parlance </param>
public record GenerateResourcesCommand(IDictionary? ResourceList, string ResourceSet, [Required(AllowEmptyStrings = true)] string? LanguageId, bool DeleteAllResourcesFirst, [Required(AllowEmptyStrings = true)] string? Industry, [Required(AllowEmptyStrings = true)] string? Customer);
