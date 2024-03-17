using idee5.Globalization.Models;

namespace idee5.Globalization.Web.Models {
    /// <summary>
    /// The resource view model.
    /// </summary>
    public record ResourceViewModel : Resource {
        private LanguageViewModel _languageItem;

        /// <summary>
        /// Gets the unique model id. Generated for the use in kendo ui components.
        /// </summary>
        /// <value>The model id</value>
        public string ModelId {
            get { return string.Format(format: "{0}{1}{2}{3}{4}", args: [ResourceSet, Id, Language, Industry, Customer]); }
        }

        /// <summary>
        /// Gets the language item. For usage in combo boxes.
        /// </summary>
        /// <value>The language item</value>
        public LanguageViewModel LanguageItem {
            get { return _languageItem != null ? _languageItem : Language == null ? null : new LanguageViewModel(Language, new System.Globalization.CultureInfo(Language).NativeName); }
            set { _languageItem = value; }
        }

        /// <summary>
        /// Gets or sets the value. HTML allowed!
        /// </summary>
        /// <value>The value</value>
        new public string Value { get; set; }
        /// <summary>
        /// Gets or sets the textfile. HTML allowed!
        /// </summary>
        /// <value>The textfile</value>
        new public string Textfile { get; set; }
    }
}