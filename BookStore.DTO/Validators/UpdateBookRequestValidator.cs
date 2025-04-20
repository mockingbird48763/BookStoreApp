using BookStore.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Validators
{
    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        private readonly UploadImageValidator _imageValidator = new();

        public UpdateBookRequestValidator()
        {
            // 僅 UploadedImage 不為 null 時，才進行格式和大小的驗證，否則直接跳過驗證
            RuleFor(x => x.UploadedImage)
                .Must(file => file == null || _imageValidator.HaveValidMimeType(file))
                .WithMessage("Only JPG, JPEG, and PNG formats are accepted.")
                .Must(file => file == null || _imageValidator.HaveValidSize(file))
                .WithMessage("The image size must not exceed 2MB.");
        }
    }
}
