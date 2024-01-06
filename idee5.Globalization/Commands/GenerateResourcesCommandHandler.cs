using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace idee5.Globalization.Commands;
/// <summary>
/// Handler to generate resources for use in the <see cref="System.Resources.ResourceWriter"/>.
/// </summary>
public class GenerateResourcesCommandHandler : ICommandHandlerAsync<GenerateResourcesCommand> {
    #region Private Fields

    private readonly IResourceUnitOfWork _unitOfWork;

    #endregion Private Fields

    #region Public Constructors

    public GenerateResourcesCommandHandler(IResourceUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Generate resources for use in the <see cref="System.Resources.ResourceWriter"/>.
    /// </summary>
    /// <param name="command">Command parameters</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    public async Task HandleAsync(GenerateResourcesCommand command, CancellationToken cancellationToken = default) {
        if (command != null && (command.ResourceList?.Count ?? 0) > 0) {
            if (command.DeleteAllResourcesFirst) { // Delete the resource set before generating it?
                await _unitOfWork.ResourceRepository.RemoveAsync(r => r.ResourceSet == command.ResourceSet && r.Language == command.LanguageId, cancellationToken).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            foreach (Resource resource in from DictionaryEntry e in command.ResourceList
                                          where e.Value != null
                                          let resource = new Resource {
                                              BinFile = null,
                                              Comment = null,
                                              Customer = command.Customer,
                                              Id = e.Key.ToString(),
                                              Industry = command.Industry,
                                              Language = command.LanguageId,
                                              ResourceSet = command.ResourceSet,
                                              Textfile = null,
                                              Value = e.Value.ToString()
                                          }
                                          select resource) {
                await _unitOfWork.ResourceRepository.UpdateOrAddAsync(resource, cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion Public Methods
}