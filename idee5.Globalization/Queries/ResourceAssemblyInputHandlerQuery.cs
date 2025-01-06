using idee5.Common;
using idee5.Globalization.Commands;

using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Queries;

/// <summary>
/// The resource assembly input handler query.
/// </summary>
/// <param name="Path"> Path to the assembly</param>
/// <param name="Industry"> The industry parlance the resouces belong to</param>
/// <param name="Customer"> The customer parlance the resources belong to</param>
/// <param name="TargetLanguage"> The language the resource file belongs to. If <c>null</c> if will be inferred from the resources file name.
/// E.g.  CommonTerms.de.resources -> language de </param>
public record ResourceAssemblyInputHandlerQuery([Required] string Path, string? Industry, string? Customer, string? TargetLanguage) : IQuery<CreateOrUpdateResourceCommand>;
