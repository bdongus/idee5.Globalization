using idee5.Common;
using idee5.Globalization.Queries;
using idee5.Globalization.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources.NetStandard;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Commands;

public class GenerateResXFilesCommandHandler : ICommandHandlerAsync<GenerateResXFilesCommand> {
    #region Private Fields

    private const string _filereftype = "System.Resources.ResXFileRef, System.Windows.Forms";

    private readonly IResourceUnitOfWork _unitOfWork;

    #endregion Private Fields

    #region Public Constructors

    public GenerateResXFilesCommandHandler(IResourceUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    #endregion Public Constructors

    #region Public Methods
    /// <summary>
    /// Dumps resources from the resource provider database to Resx resources in an ASP.NET
    /// application creating the appropriate APP_LOCAL_RESOURCES/APP_GLOBAL_RESOURCES folders and
    /// resx files.
    /// </summary>
    /// <param name="command">The command parameters</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>An waitable <see cref="Task"/></returns>
    public async Task HandleAsync(GenerateResXFilesCommand command, CancellationToken cancellationToken = default) {
        if (command != null) {
            // Retrieve all resource sets for a given industry and/or customer split into languages.
            var query = new GetAllParlanceResourcesQuery(command.LocalResources, command.CustomerId, command.IndustryId);
            var handler = new GetAllParlanceResourcesQueryHandler(_unitOfWork.ResourceRepository);
            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<Models.Resource> resources = await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<IGrouping<(string ResourceSet, string? Language), Models.Resource>> resourceSets = resources.GroupBy(r => (r.ResourceSet, r.Language));

            var separator = new char[] { ';' };
            // create a resx file for each resource set and language
            if (resourceSets != null) {
                foreach (var set in resourceSets) {
                    // create new resx file
                    string localizedExtension = ".resx";
                    if (!String.IsNullOrWhiteSpace(set.Key.Language))
                        localizedExtension = $".{set.Key.Language}.resx";
                    string resourceFilename = set.Key.ResourceSet.GenerateResourceSetPath(command.BasePhysicalPath, command.LocalResources) + localizedExtension;
                    using (var resxWriter = new ResXResourceWriter(resourceFilename)) {
                        foreach (Models.Resource item in set) {
                            ResXDataNode? node = null;
                            if (!String.IsNullOrEmpty(item.Textfile) || item.BinFile?.Length > 0) {
                                string resourcePath = new FileInfo(resourceFilename).DirectoryName;
                                string file = Path.Combine(resourcePath, item.Value);
                                if (!String.IsNullOrEmpty(item.Textfile)) {
                                    string[] tokens = item.Value.Split(separator);
                                    Encoding encode = Encoding.Default;
                                    // if there is an encoding section, use it
                                    if (tokens.Length == 3)
                                        encode = Encoding.GetEncoding(tokens[2]);

                                    // save the file
                                    File.Delete(file);
                                    File.WriteAllText(file, item.Textfile, encode);
                                    node = new ResXDataNode(item.Id, new ResXFileRef(item.Value, _filereftype, encode));
                                } else {
                                    // TODO: Use streams for async operations (Like UpdateOrAddResource of ResourceRepository
                                    File.Delete(file);
                                    File.WriteAllBytes(file, item.BinFile);
                                    node = new ResXDataNode(item.Id, new ResXFileRef(item.Value, _filereftype));
                                }
                            } else {
                                node = new ResXDataNode(item.Id, item.Value);
                            }

                            if (!String.IsNullOrEmpty(item.Comment))
                                node.Comment = item.Comment;
                            resxWriter.AddResource(node);
                        }
                    }
                }
            }
        }
    }

    #endregion Public Methods
}