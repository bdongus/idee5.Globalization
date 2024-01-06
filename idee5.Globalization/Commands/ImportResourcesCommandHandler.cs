using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Commands;

/// <summary>
/// Import a list of <see cref="Resource"/>s.
/// </summary>
public class ImportResourcesCommandHandler : ICommandHandlerAsync<ImportResourcesCommand> {
    private readonly IResourceUnitOfWork _resourceUnitOfWork;
    /// <summary>
    /// Initialize the command handler.
    /// </summary>
    /// <param name="resourceUnitOfWork">The <see cref="IResourceUnitOfWork"/> to use for saving the resources.</param>
    public ImportResourcesCommandHandler(IResourceUnitOfWork resourceUnitOfWork) {
        _resourceUnitOfWork = resourceUnitOfWork ?? throw new ArgumentNullException(nameof(resourceUnitOfWork));
    }

    /// <summary>
    /// Import a list of <see cref="Resource"/>s.
    /// </summary>
    /// <param name="command">The command parameters.</param>
    /// <param name="cancellationToken">Token to cancel the import.</param>
    public async Task HandleAsync(ImportResourcesCommand command, CancellationToken cancellationToken = default) {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
        await _resourceUnitOfWork.ResourceRepository.UpdateOrAddAsync(command.Resources, cancellationToken).ConfigureAwait(false);
        await _resourceUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
