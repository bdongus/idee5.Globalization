using idee5.Common;
using idee5.Globalization.Models;
using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Queries;

/// <summary>
/// Query parameters for retrieval of a single resource of a parlance.
/// </summary>
/// <param name="ResourceSet"> Name of the resource set. </param>
/// <param name="Id"> Resource id. </param>
/// <param name="Language"> Language id according to BCP 47 http://tools.ietf.org/html/bcp47 </param>
/// <param name="Industry"> Name of the industry parlance. </param>
/// <param name="Customer"> Name of the customer parlance. </param>
public record GetExactResourceQuery([Required, StringLength(maximumLength: 255, MinimumLength = 1)] string ResourceSet, [Required, StringLength(maximumLength: 255, MinimumLength = 1)] string Id, [Required(AllowEmptyStrings = true)] string? Language, string? Industry,  string? Customer) : IQuery<Resource?>;
