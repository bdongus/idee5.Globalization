using idee5.Common.Data;
using idee5.Globalization.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.Repositories;

public interface IResourceQueryRepository : IQueryRepository<Resource> {
    /// <summary>
    /// Search resource sets containing a given string
    /// </summary>
    /// <param name="searchValue">String to search for</param>
    /// <returns>List of found resource sets</returns>
    Task<List<string>> SearchResourceSetsAsync(string searchValue, CancellationToken cancellationToken = default);
}
