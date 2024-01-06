using idee5.Globalization.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.EFCore
{
    /// <inheritdoc />
    public class ResourceUnitOfWork : IResourceUnitOfWork
    {
        public IResourceRepository ResourceRepository { get; }

        private readonly GlobalizationDbContext _context;

        public ResourceUnitOfWork(GlobalizationDbContext context)
        {
            _context = context;
            ResourceRepository = new ResourceRepository(context);
        }

        /// <inheritdoc />
        public Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) => _context.SaveChangesAsync(cancellationToken);

        /// <inheritdoc />
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}