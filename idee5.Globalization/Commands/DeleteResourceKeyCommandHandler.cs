using idee5.Common;
using idee5.Globalization.Repositories;

using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Commands;
/// <summary>
/// The delete resource key command handler.
/// </summary>
public class DeleteResourceKeyCommandHandler : ICommandHandlerAsync<DeleteResourceKeyCommand> {
    readonly ILogger<DeleteResourceKeyCommandHandler> _logger;
    readonly IResourceUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteResourceKeyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger.</param>
    public DeleteResourceKeyCommandHandler(IResourceUnitOfWork unitOfWork, ILogger<DeleteResourceKeyCommandHandler> logger) {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(DeleteResourceKeyCommand command, CancellationToken cancellationToken = default) {
        _logger.DeletingResourceKey(command);
        await _unitOfWork.ResourceRepository.RemoveAsync(Specifications.OfResourceKey(command), cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
