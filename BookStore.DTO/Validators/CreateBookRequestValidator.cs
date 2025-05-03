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
        private readonly UploadImageValidator _imageValidator = new();

        public CreateBookRequestValidator()
        {
            RuleFor(x => x.UploadedImage)
                .Must(file => _imageValidator.HaveValidMimeType(file)).WithMessage("Only JPG, JPEG, and PNG formats are accepted.")
                .Must(file => _imageValidator.HaveValidSize(file)).WithMessage("The image size must not exceed 2MB.");
        }
    }
}
