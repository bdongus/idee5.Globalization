using idee5.Common.Data;
using idee5.Globalization.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Queries;
// TODO: create tests incl. logging checks
/// <summary>
/// The resource assembly input handler
/// </summary>
public class ResourceAssemblyInputHandler : IAsyncInputHandler<ResourceAssemblyInputHandlerQuery, CreateOrUpdateResourceCommand> {
    private readonly ILogger<ResourceAssemblyInputHandler> _logger;
    public ResourceAssemblyInputHandler(ILogger<ResourceAssemblyInputHandler> logger) {
        _logger = logger;
    }
    /// <inheritdoc/>
    public async IAsyncEnumerable<CreateOrUpdateResourceCommand> HandleAsync(ResourceAssemblyInputHandlerQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        var assembly = await Task.Run(() => Assembly.LoadFrom(query.Path), cancellationToken);
        foreach (var resname in assembly.GetManifestResourceNames()) {
            using (var stream = assembly.GetManifestResourceStream(resname)) {
                if (resname.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) {
                    var resourceSetName = Path.GetFileNameWithoutExtension(resname);
                    string? targetLanguage = query.TargetLanguage;
                    try {
                        // if the extension is a culture, use it as the language
                        string language = Path.GetExtension(resourceSetName).Trim('.');
                        CultureInfo.GetCultureInfo(language);
                        targetLanguage = language;
                        resourceSetName = Path.GetFileNameWithoutExtension(resourceSetName);
                    }
                    catch (CultureNotFoundException) {
                    }
                    using (var reader = new ResourceReader(stream)) {
                    foreach (DictionaryEntry item in reader) {
                        cancellationToken.ThrowIfCancellationRequested();
                        var key = item.Key.ToString();
                        var value = item.Value.ToString();
                        yield return new CreateOrUpdateResourceCommand {
                            Comment = null,
                            Customer = query.Customer,
                            Id = key,
                            Industry = query.Industry,
                            Language = targetLanguage,
                            ResourceSet = resourceSetName,
                            Value = value
                        };
                    }
                }
                }
            }
        }
    }
}
