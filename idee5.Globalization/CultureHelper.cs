using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace idee5.Globalization;
/// <summary>
/// Singleton offering helper methods to ease kendo ui internationalization
/// </summary>
public sealed class CultureHelper {
    // http://csharpindepth.com/Articles/General/Singleton.aspx#lazy
    private static readonly Lazy<CultureHelper> _cultureHelper = new(() => new CultureHelper());

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <value>The instance.</value>
    public static CultureHelper Instance {
        get { return _cultureHelper.Value; }
    }

    /// <summary>
    /// List of valid cultures
    /// </summary>
    /// <value>Returns the list of cultures available within the framework.</value>
    public List<string> ValidCultures { get; }

    public List<string> ImplementedCultures { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureHelper"/> class.
    /// </summary>
    private CultureHelper() {
        // first culture is the DEFAULT.
        ImplementedCultures = ConfigurationManager.AppSettings["implementedCultures"]?.Split(',').Select(s => s.Trim()).ToList() ?? [];

        ValidCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures).Select(c => c.Name).ToList();
    }

    /// <summary>
    /// Returns true if the current language is a right-to-left language. Otherwise, false.
    /// </summary>
    public static bool IsRighToLeft() => Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

    /// <summary>
    /// Gets the implemented culture.
    /// </summary>
    /// <param name="name">Culture code (e.g. en-US)</param>
    /// <returns>
    /// Returns a valid culture code based on "name" parameter. If "name" is not valid, it
    /// returns the default culture.
    /// </returns>
    public string GetImplementedCulture(string name) {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        string defaultCulture = GetDefaultCulture();
        // return the default culture if it is invalid
        if (string.IsNullOrEmpty(name) || !ValidCultures.Any(c => c.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return defaultCulture;

        // if it is implemented, accept it
        if (ImplementedCultures.Any(c => c.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return name;

        // Find a close match. For example, if you have "en-US" defined and the user requests
        // "en-GB", the function will return closest match that is "en-US" because at least the
        // language is the same (ie English)
        string match = ImplementedCultures.Find(ic => ic.StartsWith(GetNeutralCulture(name), StringComparison.OrdinalIgnoreCase));

        // else it is not implemented
        return String.IsNullOrEmpty(match) ? defaultCulture : match; // return default culture as no match found
    }

    /// <summary>
    /// Returns default culture name which is the first name declared (e.g. en-US)
    /// </summary>
    /// <returns>Name of the default culture. Or blank if there is no culture at all.</returns>
    public string GetDefaultCulture() {
        if (ImplementedCultures.Count > 0)
            return ImplementedCultures[0];
        else return String.Empty;
    }

    /// <summary>
    /// Get the culture name of the current thread
    /// </summary>
    /// <returns>Name of the current culture, e.g. "en-US"</returns>
    public static string GetCurrentCulture() => Thread.CurrentThread.CurrentCulture.Name;

    /// <summary>
    /// Get the UI culture name of the current thread
    /// </summary>
    /// <returns>Name of the current UI culture, e.g. "en-US"</returns>
    public static string GetCurrentUICulture() => Thread.CurrentThread.CurrentUICulture.Name;

    /// <summary>
    /// Get the neutral culture name of the current thread
    /// </summary>
    /// <returns>Name of the current UI culture, e.g. "en" if the culture is "en-US"</returns>
    public static string GetCurrentNeutralCulture() => GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);

    /// <summary>
    /// Get the neutral UI culture name of the current thread
    /// </summary>
    /// <returns>Name of the current UI culture, e.g. "en" if the culture is "en-US"</returns>
    public static string GetCurrentNeutralUICulture() => GetNeutralCulture(Thread.CurrentThread.CurrentUICulture.Name);

    /// <summary>
    /// Get the neutral culture part of a culture name
    /// </summary>
    /// <returns>E.g. "en" if the culture is "en-US"</returns>
    public static string GetNeutralCulture(string name) {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (!name.Contains(value: "-"))
            return name;

        return name.Split('-')[0]; // Read first part only. E.g. "en", "es"
    }
}