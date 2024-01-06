namespace idee5.Globalization.Commands;
/// <summary>
/// Command parameters for generating ResX files from a resource set
/// </summary>
/// <param name="BasePhysicalPath"> The physical path of the Web application. This path serves as the root path to write resources to.
/// Example: c:\projects\idee5.LoggingStation </param>
/// <param name="CustomerId"> Id of the customer parlance</param>
/// <param name="IndustryId"> Id of the industry parlance</param>
/// <param name="LocalResources"> NULL generates all files. "true" generates the local files, "false" the global files. </param>
public record GenerateResXFilesCommand(string BasePhysicalPath, string? CustomerId, string? IndustryId, bool? LocalResources);
