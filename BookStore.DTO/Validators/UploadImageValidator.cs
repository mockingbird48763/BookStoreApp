using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Validators
{
    public class UploadImageValidator
    {
        private readonly HashSet<string> allowedMimeTypes = ["image/jpeg", "image/png"];

        public bool HaveValidMimeType(IFormFile file)
        {
            var fileMimeType = file.ContentType.ToLower();
            return allowedMimeTypes.Contains(fileMimeType);
        }

        public bool HaveValidSize(IFormFile file)
        {
            return file.Length <= 2 * 1024 * 1024; // 2MB
        }
    }
}
