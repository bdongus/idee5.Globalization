using idee5.Common;
using idee5.Globalization.Commands;
using idee5.Globalization.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Resources.NetStandard;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;
public class ResxFileInputHandler : IQueryHandlerAsync<ResxFileInputHandlerQuery, ImportResourcesCommand> {
    public async Task<ImportResourcesCommand> HandleAsync(ResxFileInputHandlerQuery query, CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        var resultList = new List<Resource>();
        string fileDir = Path.GetDirectoryName(query.Path) + Path.DirectorySeparatorChar;
        var fi = new FileInfo(query.Path);
        if (fi.Exists) {
            string fileContent;
            using (StreamReader sr = fi.OpenText())
                fileContent = await sr.ReadToEndAsync().ConfigureAwait(false);
            using (var reader = ResXResourceReader.FromFileContents(fileContent)) {
                foreach (DictionaryEntry item in reader) {
                    cancellationToken.ThrowIfCancellationRequested();
                    var node = (ResXDataNode)item.Value;
                    string name = node.Name;
                    string comment = node.Comment;
                    string value = node.GetValue(null as ITypeResolutionService).ToString();
                    string type = node.GetValueTypeName(null as ITypeResolutionService);

                    if (string.IsNullOrEmpty(type)) {
                        // File based resources are formatted: filename;full type name
                        string[] tokens = value.Split(';');
                        if (tokens.Length > 0) {
                            string resFileName = Path.Combine(fileDir, tokens[0]);
                            if (File.Exists(resFileName))
                                value = resFileName;
                        }
                    }
                    var resource = new Resource {
                        BinFile = null,
                        Comment = comment,
                        Customer = query.Customer,
                        Id = name,
                        Industry = query.Industry,
                        Language = query.TargetLanguage,
                        ResourceSet = query.ResourceSet,
                        Textfile = null,
                        Value = value
                    };
                    resultList.Add(resource);
                }
            }
        }
        return new ImportResourcesCommand(resultList);
    }
}
