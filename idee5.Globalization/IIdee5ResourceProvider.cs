using System;

namespace idee5.Globalization;
/// <summary>
/// Resource Provider marker interface. Also provides for clearing resources.
/// </summary>
[Obsolete("This is for old full framework applications.")]
public interface IIdee5ResourceProvider {
    /// <summary>
    /// Releases all resources and forces resources to be reloaded from storage on the next GetResourceSet
    /// </summary>
    void ClearResourceCache();
}