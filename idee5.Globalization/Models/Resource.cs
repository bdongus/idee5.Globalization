using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace idee5.Globalization.Models;
/// <summary>
/// A culture specific resource supporting customer and industry parlances.
/// </summary>
public class Resource {
    #region Private Fields

    private byte[]? _binFile;

    #endregion Private Fields

    #region Public Properties
    /// <summary>
    /// Name of the resource set
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string ResourceSet { get; set; }

    /// <summary>
    /// Language id according to BCP 47 http://tools.ietf.org/html/bcp47
    /// </summary>
    [Required(AllowEmptyStrings = true)]
    public string? Language { get; set; }

    /// <summary>
    /// Name of the industry parlance
    /// </summary>
    public string? Industry { get; set; }

    /// <summary>
    /// Name of the customer parlance
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// Resource id
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string Id { get; set; }

    /// <summary>
    /// Value of the resource for the specified culture and parlance
    /// </summary>
    [Required, StringLength(maximumLength: 255, MinimumLength = 1)]
    public string Value { get; set; }

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

    #region Public Methods

    /// <inheritdoc />
    public override bool Equals(object obj) {
        return obj is Resource resource
               && EqualityComparer<byte[]>.Default.Equals(BinFile, resource.BinFile)
               && Comment == resource.Comment
               && Customer == resource.Customer
               && Id == resource.Id
               && Industry == resource.Industry
               && Language == resource.Language
               && ResourceSet == resource.ResourceSet
               && Textfile == resource.Textfile
               && Value == resource.Value;
    }

    public override int GetHashCode() {
        var hash = new System.HashCode();
        hash.Add(BinFile);
        hash.Add(Comment);
        hash.Add(Customer);
        hash.Add(Id);
        hash.Add(Industry);
        hash.Add(Language);
        hash.Add(ResourceSet);
        hash.Add(Textfile);
        hash.Add(Value);
        return hash.ToHashCode();
    }

    #endregion Public Methods
}