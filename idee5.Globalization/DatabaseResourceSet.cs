using idee5.Globalization.Repositories;
using System;
using System.Globalization;
using System.Resources;

namespace idee5.Globalization;
/// <summary>
/// This is the database driven resource set version. Each culture has a separate resource set.
/// The reading of resources is managed by the ResourceReader.
/// </summary>
public class DatabaseResourceSet : ResourceSet {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResoureSet"/> class. Inject the
    /// resource reader
    /// </summary>
    /// <param name="repository">The database context.</param>
    /// <param name="resourceSet">The resource set.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="industry">The industry.</param>
    /// <param name="customer">The customer.</param>
    public DatabaseResourceSet(IResourceRepository repository, string resourceSet, CultureInfo culture, string? industry, string? customer)
        : base(new DatabaseResourceReader(repository, resourceSet, culture?.Name ?? throw new ArgumentNullException(nameof(culture)), industry, customer)) {
    }

    /// <summary>
    /// Marker method that provides the type used for the ResourceReader.
    /// </summary>
    /// <returns></returns>
    public override Type GetDefaultReader() { return typeof(DatabaseResourceReader); }

    /// <summary>
    /// Marker method that provides the type used for a ResourceWriter.
    /// </summary>
    /// <returns></returns>
    public override Type GetDefaultWriter() { return typeof(DatabaseResourceWriter); }
}