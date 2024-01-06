namespace idee5.Globalization.Models;

/// <summary>
/// Wiew model to support language comboboxes
/// </summary>
/// <param name="Id"> ISO language id </param>
/// <param name="Name"> Native language name </param>
public record LanguageViewModel(string Id, string Name);
