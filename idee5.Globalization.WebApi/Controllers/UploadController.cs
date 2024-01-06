using idee5.Common;
using idee5.Common.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace idee5.Globalization.WebApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller {
        private readonly ILogger _logger;
        private readonly string _fileSaveLocation;

        public UploadController(ILogger logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSaveLocation = AppDomain.CurrentDomain.GetDataDirectory();
        }

        /// <summary>
        /// Posts files to be stored.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PostFileAsync(IFormFile file, CancellationToken cancellationToken = default) {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string path = Path.Combine(_fileSaveLocation, file.Name);
            _logger.LogInformation(String.Format(CultureInfo.CurrentUICulture, Properties.Resources.FileResourceSavedAt, path));
            if (file.Length > 0 && file.Name.HasValue()) {
                using (var fileStream = new FileStream(path, FileMode.Create)) {
                    await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                }
            }
            return CreatedAtAction(nameof(PostFileAsync), null);
        }
    }
}