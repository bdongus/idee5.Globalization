using idee5.Common;
using idee5.Globalization.Models;
using idee5.Globalization.Repositories;

using Microsoft.Extensions.Logging;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Commands;

/// <summary>
/// The update resource key command handler. Removes translations NOT in the given list.
/// </summary>
public class UpdateResourceKeyCommandHandler : ICommandHandlerAsync<UpdateResourceKeyCommand> {
    private readonly IResourceUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateResourceKeyCommandHandler> _logger;

    public UpdateResourceKeyCommandHandler(IResourceUnitOfWork unitOfWork, ILogger<UpdateResourceKeyCommandHandler> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(UpdateResourceKeyCommand command, CancellationToken cancellationToken = default) {
        _logger.TranslationsReceived(command.Translations.Count, command);
        Resource baseResource = new() {
            ResourceSet = command.ResourceSet,
            Id = command.Id,
            Customer = command.Customer,
            Industry = command.Industry
        };
        // first remove all missing translations
        _logger.RemovingTranslations();
        await _unitOfWork.ResourceRepository.RemoveAsync(Specifications.OfResourceKey(baseResource) & !Specifications.TranslatedTo(command.Translations.Keys), cancellationToken).ConfigureAwait(false);

        // then update or add the given translations
        foreach (var translation in command.Translations) {
            Resource rsc = baseResource with { Language = translation.Key, Value = translation.Value };
            _logger.CreateOrUpdateResource(rsc);
            await _unitOfWork.ResourceRepository.UpdateOrAddAsync(rsc, cancellationToken).ConfigureAwait(false);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
