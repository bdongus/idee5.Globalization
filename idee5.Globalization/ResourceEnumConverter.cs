using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace idee5.Globalization;
/// <summary>
/// Defines a type converter for enum values that converts enum values to and from string
/// representations using resources
/// </summary>
/// <remarks>
/// This class makes localization of display values for enums in a project easy. Simply derive a
/// class from this class and pass the ResourceManager in the constructor.
/// <code lang="C#" escaped="true">
/// class LocalizedEnumConverter : ResourceEnumConverter
/// {
///    public LocalizedEnumConverter(Type type)
///        : base(type, Properties.Resources.ResourceManager)
///    {
///    }
/// }
/// </code>
/// Then define the enum values in the resource editor. The names of the resources are simply the
/// enum value prefixed by the enum type name with an underscore separator eg MyEnum_MyValue. You
/// can then use the TypeConverter attribute to make the LocalizedEnumConverter the default
/// TypeConverter for the enums in your project.
/// </remarks>
public class ResourceEnumConverter : EnumConverter {
    #region Private Fields

    private readonly Array? _flagValues;
    private readonly bool _isFlagEnum;
    private readonly Dictionary<CultureInfo, LookupTable> _lookupTables = [];
    private readonly ResourceManager _resourceManager;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Create a new instance of the converter using translations from the given resource manager
    /// </summary>
    /// <param name="type">A <see cref="Type"/> that represents the type of enumeration to associate with this enumeration converter</param>
    /// <param name="resourceManager">The <see cref="ResourceManager"/> delivering the translations</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="resourceManager"/> is <c>null</c>.</exception>
    public ResourceEnumConverter(Type type, ResourceManager resourceManager) : base(type) {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
        object[] flagAttributes = type.GetCustomAttributes(typeof(FlagsAttribute), inherit: true);
        _isFlagEnum = flagAttributes.Length > 0;
        if (_isFlagEnum) {
            _flagValues = Enum.GetValues(type);
        }
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Convert the given enum value to string using the registered type converter
    /// </summary>
    /// <param name="value">The enum value to convert to string</param>
    /// <returns>The localized string value for the enum</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    static public string ConvertToString(Enum value) {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
        return converter.ConvertToString(value);
    }

    /// <summary>
    /// Return a list of the enum values and their associated display text for the given enum type
    /// </summary>
    /// <param name="enumType">The enum type to get the values for</param>
    /// <param name="culture">The culture to get the text for</param>
    /// <returns>
    /// A list of KeyValuePairs where the key is the enum value and the value is the text to display
    /// </returns>
    /// <remarks>
    /// This method can be used to provide localized binding to enums in ASP.NET applications.
    /// Unlike windows forms the standard ASP.NET controls do not use TypeConverters to convert
    /// from enum values to the displayed text. You can bind an ASP.NET control to the list
    /// returned by this method by setting the DataValueField to "Key" and theDataTextField to "Value".
    /// </remarks>
    static public List<KeyValuePair<Enum, string>> GetEnumValues(Type enumType, CultureInfo culture) {
        var result = new List<KeyValuePair<Enum, string>>();
        TypeConverter converter = TypeDescriptor.GetConverter(enumType);
        foreach (Enum value in Enum.GetValues(enumType)) {
            var pair = new KeyValuePair<Enum, string>(value, converter.ConvertToString(context: null, culture: culture, value: value));
            result.Add(pair);
        }
        return result;
    }

    /// <summary>
    /// Return a list of the enum values and their associated display text for the given enum
    /// type in the current UI Culture
    /// </summary>
    /// <param name="enumType">The enum type to get the values for</param>
    /// <returns>
    /// A list of KeyValuePairs where the key is the enum value and the value is the text to display
    /// </returns>
    /// <remarks>
    /// This method can be used to provide localized binding to enums in ASP.NET applications.
    /// Unlike windows forms the standard ASP.NET controls do not use TypeConverters to convert
    /// from enum values to the displayed text. You can bind an ASP.NET control to the list
    /// returned by this method by setting the DataValueField to "Key" and theDataTextField to "Value".
    /// </remarks>
    static public List<KeyValuePair<Enum, string>> GetEnumValues(Type enumType)
        => GetEnumValues(enumType, CultureInfo.CurrentUICulture);

    /// <summary>
    /// Convert string values to enum values
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed</param>
    /// <param name="value">Value to convert</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is string strval) {
            return (_isFlagEnum ?
                GetFlagValue(culture, strval) : GetValue(culture, strval)) ?? base.ConvertFrom(context, culture, value);
        } else {
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Convert the enum value to a string
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed</param>
    /// <param name="value">Value to convert</param>
    /// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to</param>
    public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
        if (value != null && destinationType == typeof(string)) {
            return _isFlagEnum ? GetFlagValueText(culture, value) : GetValueText(culture, value);
        } else {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Return true if the given value can be represented using a single bit
    /// </summary>
    /// <param name="value"></param>
    private static bool IsSingleBitValue(ulong value) {
        return value switch {
            0 => false,
            1 => true,
            _ => (value & (value - 1)) == 0,
        };
    }

    /// <summary>
    /// Return the Enum value for a flagged enum
    /// </summary>
    /// <param name="culture">The culture to convert using</param>
    /// <param name="text">The text to convert</param>
    /// <returns>The enum value</returns>
    private object? GetFlagValue(CultureInfo culture, string text) {
        LookupTable lookupTable = GetLookupTable(culture);
        string[] textValues = text.Split(',');
        ulong result = 0;
        foreach (string textValue in textValues) {
            string trimmedTextValue = textValue.Trim();
            if (!lookupTable.TryGetValue(trimmedTextValue, out object value)) {
                return null;
            }
            result |= Convert.ToUInt32(value, culture);
        }
        return Enum.ToObject(EnumType, result);
    }

    /// <summary>
    /// Return the text to display for a flag value in the given culture
    /// </summary>
    /// <param name="culture">The culture to get the text for</param>
    /// <param name="value">The flag enum value to get the text for</param>
    /// <returns>The localized text</returns>
    private string? GetFlagValueText(CultureInfo culture, object value) {
        // if there is a standard value then use it
        if (Enum.IsDefined(value.GetType(), value)) {
            return GetValueText(culture, value);
        }

        // otherwise find the combination of flag bit values that makes up the value
        ulong lValue = Convert.ToUInt32(value, culture);
        string? result = null;
        if (_flagValues != null) {
            foreach (object flagValue in _flagValues) {
                ulong lFlagValue = Convert.ToUInt32(flagValue, culture);
                if (IsSingleBitValue(lFlagValue) && (lFlagValue & lValue) == lFlagValue) {
                    string valueText = GetValueText(culture, flagValue);
                    result = result == null ? valueText : string.Format(culture, format: "{0}, {1}", arg0: result, arg1: valueText);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Get the lookup table for the given culture (creating if necessary)
    /// </summary>
    /// <param name="culture"></param>
    private LookupTable GetLookupTable(CultureInfo culture) {
        culture ??= CultureInfo.CurrentCulture;

        if (!_lookupTables.TryGetValue(culture, out LookupTable result)) {
            result = new LookupTable();
            foreach (object value in GetStandardValues()) {
                string text = GetValueText(culture, value);
                if (text != null) {
                    result.Add(text, value);
                }
            }
            _lookupTables.Add(culture, result);
        }
        return result;
    }

    /// <summary>
    /// Return the Enum value for a simple (non-flagged enum)
    /// </summary>
    /// <param name="culture">The culture to convert using</param>
    /// <param name="text">The text to convert</param>
    /// <returns>The enum value</returns>
    private object GetValue(CultureInfo culture, string text) {
        LookupTable lookupTable = GetLookupTable(culture);
        lookupTable.TryGetValue(text, out object result);
        return result;
    }

    /// <summary>
    /// Return the text to display for a simple value in the given culture
    /// </summary>
    /// <param name="culture">The culture to get the text for</param>
    /// <param name="value">The enum value to get the text for</param>
    /// <returns>The localized text</returns>
    private string GetValueText(CultureInfo culture, object value) {
        Type type = value.GetType();
        string resourceName = String.Format(culture, format: "{0}_{1}", arg0: type.Name, arg1: value.ToString());
        return _resourceManager.GetString(resourceName, culture) ?? resourceName;
    }

    #endregion Private Methods

    #region Private Classes

    private class LookupTable : Dictionary<string, object> {
    }

    #endregion Private Classes
}