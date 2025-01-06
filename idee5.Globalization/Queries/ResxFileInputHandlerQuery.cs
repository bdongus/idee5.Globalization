using idee5.Common;
using idee5.Globalization.Commands;

using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Queries;

/// <summary>
/// Input handler parameters
/// </summary>
/// <param name="Path"> Path to the resource file</param>
/// <param name="ResourceSet"> Name of the resource set. If <c>null</c> or empty, the file name is used to determine the name.
/// E.g. CommonTerms-de.resx -> resource set CommonTerms </param>
/// <param name="Industry"> The industry parlance the resouce file belongs to</param>
/// <param name="Customer"> The customer parmance the resource file belongs to</param>
/// <param name="TargetLanguage"> The language the resource file belongs to. If <c>null</c> if will be inferred from the file name.
/// E.g.  CommonTerms.de.resx -> language de </param>
public record ResxFileInputHandlerQuery([Required] string Path, string ResourceSet, string? Industry, string? Customer, string? TargetLanguage) : IQuery<CreateOrUpdateResourceCommand>;
