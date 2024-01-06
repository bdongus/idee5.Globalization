using System;
using System.ComponentModel;
using System.Configuration;

namespace idee5.Globalization.Configuration;
/// <summary>
/// Custom config section handler <see cref="System.Configuration.ConfigurationSection"/>
/// <example> &lt;configSections&gt;
/// &lt;section name="ResourceProvider"
/// type="idee5.Globalization.Configuration.ResourceProviderSection,idee5.Globalization"
/// requirePermission="false" /&gt; &lt;/configSections&gt; </example>
/// </summary>
[Obsolete("This is for old full framework applications.")]
public class ResourceProviderSection : ConfigurationSection {
    [Description("The industry id used to search resources."),
    ConfigurationProperty("industryId", DefaultValue = null, IsRequired = false)]
    public string? IndustryId {
        get { return this["industryId"] as string; }
        set { this["industryId"] = value; }
    }

    [Description("The customer id used to resources. Most specific level before searching locales."),
    ConfigurationProperty("customerId", DefaultValue = null, IsRequired = false)]
    public string? CustomerId {
        get { return this["customerId"] as string; }
        set { this["customerId"] = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceProviderSection"/> class.
    /// </summary>
    public ResourceProviderSection() {
    }
}