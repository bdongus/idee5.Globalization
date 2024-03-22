using idee5.Common.Data;
using idee5.Globalization.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Repositories;

/// <summary>
/// Partial, data access agnostic, implementation of the resource respository.
/// </summary>
public abstract class AResourceRepository : ACompositeKeyRepository<Resource>, IResourceRepository {
    #region Protected Fields

    protected readonly string[] _bitmapExtensions = ["GIF", "JPG", "PNG", "BMP", "JPEG"];
    protected readonly string[] _textExtensions = ["TXT", "XML", "CSS", "JS", "HTM", "HTML", "CONFIG", "CSHTML", "JSON"];


    #endregion Protected Fields

    #region Public Methods

    /// <inheritdoc/>
    public abstract Task<List<string>> SearchResourceSetsAsync(string searchValue, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public override async Task UpdateOrAddAsync(Resource resource, CancellationToken cancellationToken = default) {
        // Validation
        if (resource == null)
            throw new ArgumentNullException(nameof(resource));
        if (string.IsNullOrEmpty(resource.Id)) {
            throw new ArgumentException(nameof(resource.Id));
        }
        if (resource.Value == null) {
            throw new ArgumentNullException(nameof(resource.Value));
        }

        resource.Language ??= String.Empty;
        if (resource.Language.Length > 0)
            _ = new CultureInfo(resource.Language); // throws an exception if not valid.

        // default values
        resource.Industry ??= String.Empty;
        resource.Customer ??= String.Empty;

        var fi = new FileInfo(resource.Value);
        // If the resource is a file, store it in the database.
        if (fi.Exists) {
            string extension = fi.Extension.ToUpperInvariant().TrimStart(trimChars: ['.']);
            // if the file name starts with a guid, remove it, as it was generated only to manage an upload
            int guidLength = fi.Name.IndexOf(value: "_", StringComparison.Ordinal) - 1;
            string fileName = fi.Name;
            if (guidLength > 0 && Guid.TryParse(fi.Name.Substring(0, guidLength), out Guid resultGuid))
                fileName = fi.Name.Substring(guidLength + 2);
            cancellationToken.ThrowIfCancellationRequested();
            if (_textExtensions.Any(extension.Contains)) { // file type containing text
                //using (StreamReader sr = new StreamReader(newRes.Value, Encoding.Default, detectEncodingFromByteOrderMarks: true))
                using (StreamReader sr = fi.OpenText())
                    resource.Textfile = await sr.ReadToEndAsync().ConfigureAwait(false);
                var sb = new StringBuilder();
                sb.Append(fi.Name).Append(";").Append(typeof(string).AssemblyQualifiedName).Append(";").Append(Encoding.Default.HeaderName);
                resource.Value = sb.ToString();
            } else { // all others are binary data
                using (FileStream fr = fi.OpenRead()) {
                    int length = (int)fi.Length;
                    resource.BinFile = new byte[length];
                    await fr.ReadAsync(resource.BinFile, 0, length).ConfigureAwait(false);
                }
                string resourceType = _bitmapExtensions.Any(extension.Contains) ? typeof(Bitmap).AssemblyQualifiedName :
                                        (extension == "ICO") ? typeof(Icon).AssemblyQualifiedName : typeof(byte[]).AssemblyQualifiedName;
                resource.Value = fileName + ";" + resourceType;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        bool isUpdate = await ExistsAsync(r => r.Language == resource.Language && r.Id == resource.Id && r.ResourceSet == resource.ResourceSet && r.Industry == resource.Industry && r.Customer == resource.Customer, cancellationToken).ConfigureAwait(false);
        if (isUpdate)
            Update(resource);
        else
            Add(resource);
        // Delete the file after importing it. Save some time by not waiting for the delete to complete.
#pragma warning disable CS4014
        if (fi.Exists) Task.Run(() => fi.Delete()).ConfigureAwait(false);
#pragma warning restore CS4014
    }

    #endregion Public Methods
}