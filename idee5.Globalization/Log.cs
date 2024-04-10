using idee5.Globalization.Models;

using Microsoft.Extensions.Logging;

namespace idee5.Globalization;
internal static partial  class Log {
    [LoggerMessage(1, LogLevel.Warning, "Resx file '{FilePath}' not found!")]
    public static partial void ResxFileNotFound(this ILogger logger, string FilePath);
    [LoggerMessage(2,LogLevel.Debug, "Received {Count} translations for {Resource}")]
    public static partial void TranslationsReceived(this ILogger logger, int Count, ResourceKey Resource);

    [LoggerMessage(3, LogLevel.Debug, "Removing translations ...")]
    public static partial void RemovingTranslations(this ILogger logger);
    [LoggerMessage(4, LogLevel.Debug, "Create or update Resource: {Resource}")]
    public static partial void CreateOrUpdateResource(this ILogger logger, Resource Resource);
    [LoggerMessage(5, LogLevel.Information, "Deleting Resource key: {Key}")]
    public static partial void DeletingResourceKey(this ILogger logger, ResourceKey Key);
}
