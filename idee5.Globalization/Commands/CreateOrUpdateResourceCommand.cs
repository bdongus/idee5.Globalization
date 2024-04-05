using idee5.Globalization.Models;

using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Commands;
/// <summary>
/// The create or update resource command
/// </summary>
public record CreateOrUpdateResourceCommand : ResourceKey {
    /// <summary>
    /// Language id according to BCP 47 http://tools.ietf.org/html/bcp47
    /// </summary>
    [Required(AllowEmptyStrings = true)]
    public string? Language { get; set; }

    /// <summary>
    /// Value of the resource for the specified culture and parlance
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string Value { get; set; } = "";

    /// <summary>
    /// Additional information. Mostly used to create semantic context to simplify translations
    /// </summary>
    public string? Comment { get; set; }
}
