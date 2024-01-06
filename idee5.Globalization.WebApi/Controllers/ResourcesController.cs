using idee5.Globalization.Models;
using idee5.Globalization.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSpecifications;
using System;
using System.Threading;
using System.Threading.Tasks;
using static idee5.Globalization.Specifications;

namespace idee5.Globalization.WebApi.Controllers {
    /// <summary>
    /// Web api controller offering access to the idee5 localization resources.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase {
        private readonly IResourceUnitOfWork _resourceUnitOfWork;
        private readonly IResourceQueryRepository _resourceQueryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesController" /> class for unit tests.
        /// </summary>
        /// <param name="resourceUnitOfWork">The writeable repository.</param>
        /// <param name="resourceQueryRepository">The read only respoistory.</param>
        public ResourcesController(IResourceUnitOfWork resourceUnitOfWork, IResourceQueryRepository resourceQueryRepository) {
            _resourceUnitOfWork = resourceUnitOfWork ?? throw new ArgumentNullException(nameof(resourceUnitOfWork));
            _resourceQueryRepository = resourceQueryRepository ?? throw new ArgumentNullException(nameof(resourceQueryRepository));
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="set">The set.</param>
        /// <param name="lang">The lang.</param>
        /// <param name="industry">The industry.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The requested resource</returns>
        [HttpGet]
        public async Task<Resource?> Get(string id, string set, string lang, string industry, string customer, CancellationToken cancellationToken) {
            ASpec<Resource> filter = ResourceId(id) & InResourceSet(set) & OfLanguage(lang) & CustomerParlance(customer) & IndustryParlance(industry);
            return await _resourceQueryRepository.GetSingleAsync(filter, cancellationToken).ConfigureAwait(false);
        }

        // POST: api/ResourcesController
        /// <summary>
        /// Adds the given resource to the repository.
        /// </summary>
        /// <param name="value">The resource to be added.</param>
        [HttpPost]
        public async Task<ActionResult<Resource>> Post([FromBody]Resource value, CancellationToken cancellationToken) {
            _resourceUnitOfWork.ResourceRepository.Add(value);
            await _resourceUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(this.Post), value);
        }

        /// <summary>
        /// Updates the given resource to the repository.
        /// </summary>
        /// <param name="value">The resource to be updated.</param>
        /// <param name="cancellationToken"></param>
        [HttpPut]
        public async Task<ActionResult<Resource>> Put([FromBody]Resource value, CancellationToken cancellationToken) {
            await _resourceUnitOfWork.ResourceRepository.UpdateOrAddAsync(value, cancellationToken).ConfigureAwait(false);
            await _resourceUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Deletes the specified resource from the repository.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        [HttpDelete]
        public async Task<ActionResult<Resource>> Delete(Resource value, CancellationToken cancellationToken) {
            _resourceUnitOfWork.ResourceRepository.Remove(value);
            await _resourceUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return NoContent();
        }
    }
}