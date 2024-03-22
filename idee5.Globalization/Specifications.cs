using idee5.Globalization.Models;
using NSpecifications;
using System;

namespace idee5.Globalization;
/// <summary>
/// Often used specifications.
/// </summary>
public static class Specifications {
    #region Public Fields

    /// <summary>
    /// Check if the language is NULL or empty
    /// </summary>
    public static readonly ASpec<Resource> NeutralLanguage = new Spec<Resource>(r => String.IsNullOrEmpty(r.Language));

    /// <summary>
    /// Check if <see cref="Resource.BinFile"/> and <see cref="Resource.Textfile"/> are NULL.
    /// </summary>
    public static readonly ASpec<Resource> StringResource = new Spec<Resource>(r => r.BinFile == null && r.Textfile == null);

    /// <summary>
    /// Check if <see cref="Resource.Textfile"./> is not NULL
    /// </summary>
    public static readonly ASpec<Resource> TextfileResource = new Spec<Resource>(r => r.Textfile != null);

    /// <summary>
    /// Check if <see cref="Resource.BinFile"/> is not NULL
    /// </summary>
    public static readonly ASpec<Resource> BinaryFileResource = new Spec<Resource>(r => r.BinFile != null);

    /// <summary>
    /// Check if <see cref="Resource.Industry"/> is NULL or empty
    /// </summary>
    public static readonly ASpec<Resource> IndustryNeutral = new Spec<Resource>(r => String.IsNullOrEmpty(r.Industry));

    /// <summary>
    /// Check if <see cref="Resource.Customer"/> is NULL or empty
    /// </summary>
    public static readonly ASpec<Resource> CustomerNeutral = new Spec<Resource>(r => String.IsNullOrEmpty(r.Customer));

    /// <summary>
    /// Check if <see cref="Resource.ResourceSet"/> contains a dot (".")
    /// </summary>
    public static readonly ASpec<Resource> LocalResources = new Spec<Resource>(r => r.ResourceSet.Contains("."));

    #endregion Public Fields

    #region Public Methods

    /// <summary>
    /// Checks if the resource is <see cref="CustomerNeutral"/> or of the customer parlance
    /// </summary>
    /// <param name="customerId">Customer parlance id</param>
    /// <returns>The combined <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> CustomerParlance(string? customerId) => CustomerNeutral | new Spec<Resource>(r => r.Customer == customerId);

    /// <summary>
    /// Checks if the resource is <see cref="IndustryNeutral"/> or of the industry parlance
    /// </summary>
    /// <param name="industryId">Industry parlance id</param>
    /// <returns>The combined <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> IndustryParlance(string? industryId) => IndustryNeutral | new Spec<Resource>(r => r.Industry == industryId);

    /// <summary>
    /// Checks if the <see cref="Resource"/> is of the given <see cref="Resource.Language"/>
    /// </summary>
    /// <param name="languageId">The BCP 47 language id</param>
    /// <returns>The new <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> OfLanguage(string? languageId) => new Spec<Resource>(r => r.Language == languageId);

    /// <summary>
    /// Checks if the <see cref="Resource"/> is of the given <see cref="Resource.Language"/> or a subtag of it.
    /// It supports up to three tags depth. Like primary, extended and region tags.
    /// </summary>
    /// <example>de-CH-1901 (the variant of German orthography dating from the 1901 reforms, as seen in Switzerland).
    /// zh-Hant-HK (Traditional Chinese as used in Hong Kong).
    /// </example>
    /// <param name="languageId">The BCP 47 language id</param>
    /// <returns>The new <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> OfLanguageOrFallback(string? languageId) => new Spec<Resource>(r => r.Language == languageId
        || (languageId != null && (
            r.Language == languageId.Remove(languageId.LastIndexOf('-') < 0 ? 0 : languageId.LastIndexOf('-'))
            || r.Language == languageId.Remove(languageId.IndexOf('-') < 0 ? 0 : languageId.IndexOf('-'))
        ))
        || String.IsNullOrEmpty(r.Language)
    );

    /// <summary>
    /// Check if the <see cref="Resource"/> is in the given <see cref="Resource.ResourceSet"/>
    /// </summary>
    /// <param name="resourceSet">Resource set to check for</param>
    /// <returns>The new <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> InResourceSet(string resourceSet) => new Spec<Resource>(r => r.ResourceSet == resourceSet);

    /// <summary>
    /// Check if the <see cref="Resource"/> has the given <see cref="Resource.Id"/>
    /// </summary>
    /// <param name="id">Id to check for</param>
    /// <returns>The new <see cref="Spec{Resource}"/></returns>
    public static ASpec<Resource> ResourceId(string id) => new Spec<Resource>(r => r.Id == id);

    /// <summary>
    /// Search the for a value in the resource set property
    /// </summary>
    /// <param name="searchValue">The search value.</param>
    /// <returns>An ASpec</returns>
    public static ASpec<Resource> ResourceSetContains(string searchValue) => new Spec<Resource>(r =>r.ResourceSet.Contains(searchValue));

    /// <summary>
    /// Search the for a value in the resource set, id, value,industry, customer or comment
    /// </summary>
    /// <param name="searchValue">The search value.</param>
    /// <returns>An ASpec</returns>
    public static ASpec<Resource> Contains(string searchValue) => new Spec<Resource>(r =>
        r.ResourceSet.Contains(searchValue)
        || r.Id.Contains(searchValue)
        || r.Value.Contains(searchValue)
        || (r.Industry != null && r.Industry.Contains(searchValue))
        || (r.Customer != null && r.Customer.Contains(searchValue))
        || (r.Comment !=null && r.Comment.Contains(searchValue))
    );

    /// <summary>
    /// Search the for a value in the resource set, id, value,industry, customer or comment
    /// </summary>
    /// <param name="resourceSet">Resource set to search in.</param>
    /// <param name="searchValue">The search value.</param>
    /// <returns>An ASpec</returns>
    public static ASpec<Resource> ContainsInResourceSet(string resourceSet, string searchValue) => new Spec<Resource>(r =>
        r.ResourceSet == resourceSet
        && (r.Id.Contains(searchValue)
        || r.Value.Contains(searchValue)
        || (r.Industry != null && r.Industry.Contains(searchValue))
        || (r.Customer != null && r.Customer.Contains(searchValue))
        || (r.Comment !=null && r.Comment.Contains(searchValue)))
    );
    #endregion Public Methods
}