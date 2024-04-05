using idee5.Common;
using idee5.Common.Data;
using idee5.Globalization.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Resources.NetStandard;
using System.Runtime.CompilerServices;
using System.Threading;

namespace idee5.Globalization.Queries;
/// <summary>
/// The RESX file input handler
/// </summary>
public class ResxFileInputHandler : IAsyncInputHandler<ResxFileInputHandlerQuery, CreateOrUpdateResourceCommand> {
    private readonly ILogger<ResxFileInputHandler> _logger;

    public ResxFileInputHandler(ILogger<ResxFileInputHandler> logger) {
        _logger = logger;
    }
    /// <inheritdoc/>
    public async IAsyncEnumerable<CreateOrUpdateResourceCommand> HandleAsync(ResxFileInputHandlerQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        var fi = new FileInfo(query.Path);
        if (fi.Exists) {
            string fileContent;
            using (StreamReader sr = fi.OpenText())
                fileContent = await sr.ReadToEndAsync().ConfigureAwait(false);
            using (var reader = ResXResourceReader.FromFileContents(fileContent)) {
                reader.UseResXDataNodes = true;
                foreach (DictionaryEntry item in reader) {
                    cancellationToken.ThrowIfCancellationRequested();
                    var node = (ResXDataNode)item.Value;
                    string name = node.Name;
                    string comment = node.Comment;
                    string value = node.FileRef?.FileName.HasValue() == true
                        ? node.FileRef.FileName
                        : node.GetValue(null as ITypeResolutionService).ToString();
                    yield return new CreateOrUpdateResourceCommand {
                        Comment = comment,
                        Customer = query.Customer,
                        Id = name,
                        Industry = query.Industry,
                        Language = query.TargetLanguage,
                        ResourceSet = query.ResourceSet,
                        Value = value
                    };
                }
            }
        }
        else {
            _logger.ResxFileNotFound(query.Path);
        }
    }
}
