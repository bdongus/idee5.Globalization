namespace idee5.Globalization.Models;

/// <summary>
/// The translation
/// </summary>
/// <param name="Language">Language id according to BCP 47 http://tools.ietf.org/html/bcp47</param>
/// <param name="Value">Value of the resource for the specified culture and parlance</param>
/// <param name="Comment">Additional information. Mostly used to create semantic context to simplify translations</param>
public record Translation(string Language, string Value, string? Comment = null);
