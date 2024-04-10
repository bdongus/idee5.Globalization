using System.ComponentModel.DataAnnotations;

namespace idee5.Globalization.Models;

/// <summary>
/// The resource key.
/// </summary>
public record ResourceKey {
    /// <summary>
    /// Name of the resource set
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string ResourceSet { get; set; } = nameof(ResourceSet);

    /// <summary>
    /// Resource id
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string Id { get; set; } = nameof(Id);

    /// <summary>
    /// Name of the industry parlance
    /// </summary>
    public string? Industry { get; set; }

    /// <summary>
    /// Name of the customer parlance
    /// </summary>
    public string? Customer { get; set; }
}
