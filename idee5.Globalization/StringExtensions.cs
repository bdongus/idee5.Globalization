using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace idee5.Globalization;

public static class StringExtensions {
    private const string _appLocalResources = "App_LocalResources";
    private const string _backslash = "\\";

    /// <summary>
    /// Generates the resource file path with file extension.
    /// </summary>
    /// <param name="resourceSet">The resource set.</param>
    /// <param name="localResources">
    /// null generates general path. "true" generates the local resources path, "false" the
    /// global resources path.
    /// </param>
    public static string GenerateResourceSetPath(this string resourceSet, string basePhysicalPath, bool? localResources = null) {
        if (resourceSet == null)
            throw new ArgumentNullException(nameof(resourceSet));
        // TODO: get rid of the multiple basephysicalpath logic, can be improved in .net core 3
        if (basePhysicalPath?.EndsWith(_backslash, StringComparison.OrdinalIgnoreCase) == false)
            basePhysicalPath += _backslash;

        // Make sure our slashes are correct
        string path = resourceSet.Replace(oldValue: "/", newValue: _backslash);
        // if it is a local resource, inject the local resources path the detection logic is the
        // same used in the resource repository
        if ((localResources == null && path.Contains(value: '.')) || localResources == true) {
            if (!path.Contains(_appLocalResources))
                path = path.Insert(path.LastIndexOf(value: '\\') + 1, _appLocalResources + _backslash);
            else
                path = string.Format(CultureInfo.InvariantCulture, "App_GlobalResources\\{0}", arg0: path);
        }

        path = Path.Combine(basePhysicalPath, path);
        var fi = new FileInfo(path);
        if (!fi.Directory.Exists)
            fi.Directory.Create();
        return path;
    }
}