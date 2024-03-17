using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace idee5.Globalization.Models;
/// <summary>
/// A culture specific resource supporting customer and industry parlances.
/// </summary>
public record Resource : ResourceKey{
    #region Private Fields

    private byte[]? _binFile;

    #endregion Private Fields

    #region Public Properties

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

    /// <summary>
    /// For imports this is empty and the <see cref="Value"/> property holds the file path to import.
    /// The file name is either the real one or can be preceeded by '<see cref="System.Guid"/>_'.
    /// </summary>
    public string? Textfile { get; set; }

    /// <summary>
    /// For imports this is empty and the <see cref="Value"/> property holds the file path to import.
    /// The file name is either the real one or can be preceeded by '<see cref="System.Guid"/>_'.
    /// </summary>
    public byte[]? BinFile {
        get { return _binFile; }
        set {
            if (value?.Length == 0) value = null; // otherwise BinFile is never null
            if (_binFile != null && value != null && !_binFile.SequenceEqual(value) || _binFile != value) {
                _binFile = value;
            }
        }
    }

    #endregion Public Properties
}