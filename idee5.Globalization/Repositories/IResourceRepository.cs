using idee5.Common.Data;
using idee5.Globalization.Models;

namespace idee5.Globalization.Repositories;

public interface IResourceRepository : ICompositeKeyRepository<Resource>, IResourceQueryRepository {
}