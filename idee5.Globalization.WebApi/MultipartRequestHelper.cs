using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;

namespace idee5.Globalization.WebApi {
    public static class MultipartRequestHelper {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit) {
            ArgumentNullException.ThrowIfNull(contentType);

            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary)) throw new InvalidDataException(Properties.Resources.MissingContentTypeBoundary);

            if (boundary.Length > lengthLimit) throw new InvalidDataException(string.Format(CultureInfo.CurrentUICulture, Properties.Resources.MultipartBoundaryLengthLimitExceeded, lengthLimit));

            return boundary;
        }

        public static bool IsMultipartContentType(string contentType) {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition) {
            // Content-Disposition: form-data; name="key";
            return contentDisposition?.DispositionType.Equals("form-data", StringComparison.Ordinal) == true
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition) {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition?.DispositionType.Equals("form-data", StringComparison.Ordinal) == true
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}
