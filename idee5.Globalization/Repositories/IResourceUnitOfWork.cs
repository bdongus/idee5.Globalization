using idee5.Common.Data;

namespace idee5.Globalization.Repositories;

/// <summary>
/// Unit of work for Globalizaion commands.
/// </summary>
public interface IResourceUnitOfWork : IUnitOfWork {
    IResourceRepository ResourceRepository { get; }
}