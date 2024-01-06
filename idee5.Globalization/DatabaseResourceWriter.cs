using idee5.Globalization.Commands;
using idee5.Globalization.Properties;
using idee5.Globalization.Repositories;
using System;
using System.Collections;
using System.Resources;

namespace idee5.Globalization;
/// <summary>
/// The database resource writer. Luckily never called within the .NET framework. So we don't
/// worry about the standard constructor. For further information see. http://flylib.com/books/en/3.147.1.143/1/
/// </summary>
public class DatabaseResourceWriter : IDisposable, IResourceWriter {
    private readonly IResourceUnitOfWork _resourceUnitOfWork;
    private readonly string _resourceSet;
    private readonly string _language;
    private readonly string? _industry;
    private readonly string? _customer;

    /// <summary>
    /// List of resources we want to add
    /// </summary>
    private readonly IDictionary _resourceList;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResourceWriter"/> class
    /// </summary>
    /// <param name="resourceUnitOfWork">The db context</param>
    /// <param name="resourceSet">The resource set</param>
    /// <param name="language">The languageId</param>
    /// <param name="industry">The industryId</param>
    /// <param name="customer">The customerId</param>
    public DatabaseResourceWriter(IResourceUnitOfWork resourceUnitOfWork, string resourceSet, string language, string? industry, string? customer) {
        _resourceList = new Hashtable();
        _resourceUnitOfWork = resourceUnitOfWork;
        _resourceSet = resourceSet;
        _language = language;
        _industry = industry;
        _customer = customer;
    }

    /// <summary>
    /// Override that reads existing resources into the list
    /// </summary>
    /// <param name="reader">The reader</param>
    /// <param name="resourceUnitOfWork">The db context</param>
    /// <param name="resourceSet">The resource set</param>
    /// <param name="language">The language</param>
    /// <param name="industry">The industry</param>
    /// <param name="customer">The customer</param>
    public DatabaseResourceWriter(IResourceReader reader, IResourceUnitOfWork resourceUnitOfWork, string resourceSet, string language, string? industry, string? customer) : this(resourceUnitOfWork, resourceSet, language, industry, customer) {
        _resourceList = (IDictionary)reader;
    }

    /// <summary>
    /// Closes the resource writer after releasing any resources associated with it
    /// </summary>
    public void Close() { Flush(); }

    /// <summary>
    /// Releases all resources, ensuring all the data has been written
    /// </summary>
    private void Flush() {
        if (_resourceList != null)
            Generate();
    }

    /// <summary>
    /// Adds a resource to the list of resources to be written to an output file or output stream
    /// </summary>
    /// <param name="name">The resourceId of the resource</param>
    /// <param name="value">The value of the resource</param>
    public void AddResource(string name, object value) {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (_resourceList == null)
            throw new InvalidOperationException(Resources.NoResources);

        _resourceList[name] = value;
    }

    /// <summary>
    /// Adds a resource to the list of resources to be written to an output file or output stream
    /// </summary>
    /// <param name="name">The resourceId of the resource</param>
    /// <param name="value">The value of the resource</param>
    public void AddResource(string name, string value) { AddResource(name, (Object)value); }

    /// <summary>
    /// Adds a resource to the list of resources to be written to an output file or output stream
    /// </summary>
    /// <param name="name">The resourceId of the resource</param>
    /// <param name="value">The value of the resource</param>
    public void AddResource(string name, byte[] value) { AddResource(name, (Object)value); }

    /// <summary>
    /// Writes all the resources added by the AddResource method to the output file or stream
    /// </summary>
    public void Generate() { Generate(deleteAllRowsFirst: false); }

    /// <summary>
    /// Writes all resources out to the resource store
    /// </summary>
    /// <param name="deleteAllRowsFirst">Flag that allows deleting all
    /// resources for a given culture and basename. To get 'clean set' of resource
    /// with no orphaned values.</param>
    public void Generate(bool deleteAllRowsFirst) {
        var cmd = new GenerateResourcesCommand(_resourceList, _resourceSet, _language, deleteAllRowsFirst, _industry, _customer);
        GenerateResourcesCommandHandler handler = new GenerateResourcesCommandHandler(_resourceUnitOfWork);
        // synchronous call
        handler.HandleAsync(cmd).GetAwaiter().GetResult();
    }

    #region IDisposable Support
    private bool _disposedValue; // Dient zur Erkennung redundanter Aufrufe.

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                Flush();
            }

            _disposedValue = true;
        }
    }

    // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}