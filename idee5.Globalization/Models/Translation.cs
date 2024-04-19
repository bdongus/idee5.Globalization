namespace idee5.Globalization.Models;

/// <summary>
/// The translation
/// </summary>
public record Translation {
    /// <summary>
    /// Create a new translation
    /// </summary>
    /// <param name="language">Language id according to BCP 47 http://tools.ietf.org/html/bcp47</param>
    /// <param name="value">Value of the resource for the specified culture and parlance</param>
    /// <param name="comment">Additional information. Mostly used to create semantic context to simplify translations</param>
    public Translation(string language, string value, string? comment = null) {
        Language = language;
        Value = value;
        Comment = comment;
    }
    /// <summary>
    /// Language id according to BCP 47 http://tools.ietf.org/html/bcp47
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Value of the resource for the specified culture and parlance
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Additional information. Mostly used to create semantic context to simplify translations
    /// </summary>
    public string? Comment { get; set; }
}