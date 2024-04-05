using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Commands;

/// <summary>
/// The create or update resource command handler.
/// </summary>
public class CreateOrUpdateResourceCommandHandler : ICommandHandlerAsync<CreateOrUpdateResourceCommand> {
    private readonly IResourceUnitOfWork _resourceUnitOfWork;

    public CreateOrUpdateResourceCommandHandler(IResourceUnitOfWork resourceUnitOfWork) {
        _resourceUnitOfWork = resourceUnitOfWork;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(CreateOrUpdateResourceCommand command, CancellationToken cancellationToken = default) {
        if (command == null)
            throw new ArgumentNullException(nameof(command));
        Resource res = new() {
            BinFile = null,
            Comment = command.Comment,
            Customer = command.Customer,
            Id = command.Id,
            Industry = command.Industry,
            Language = command.Language,
            ResourceSet = command.ResourceSet,
            Textfile = null,
            Value = command.Value
        };
        await _resourceUnitOfWork.ResourceRepository.UpdateOrAddAsync(res, cancellationToken).ConfigureAwait(false);
        await _resourceUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}