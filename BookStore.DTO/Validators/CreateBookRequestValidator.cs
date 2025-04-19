using BookStore.DTO.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        private readonly HashSet<string> allowedMimeTypes = ["image/jpeg", "image/png"];

        public CreateBookRequestValidator()
        {
            RuleFor(x => x.UploadedImage)
                .Must(HaveValidMimeType).WithMessage("Only JPG, JPEG, and PNG formats are accepted.")
                .Must(HaveValidSize).WithMessage("The image size must not exceed 2MB.");
        }

        private bool HaveValidMimeType(IFormFile file)
        {
            var fileMimeType = file.ContentType.ToLower();
            if (!allowedMimeTypes.Contains(fileMimeType))
            {
                return false;
            }
            return true;
        }

        private bool HaveValidSize(IFormFile file)
        {
            return file.Length <= 2 * 1024 * 1024;
        }
    }
}
