using Microsoft.Extensions.Logging;

namespace idee5.Globalization;
internal static partial  class Log {
    [LoggerMessage(1, LogLevel.Warning, "Resx file '{FilePath}' not found!")]
    public static partial void ResxFileNotFound(this ILogger logger, string FilePath);
}
