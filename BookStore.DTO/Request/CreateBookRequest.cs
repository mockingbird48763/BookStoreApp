using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookStore.DTO.Request
{
    public class CreateBookRequest : IValidatableObject
    {
        /// <example>123-4-567890-000</example>
        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "ISBN must be exactly 16 characters.")]
        [RegularExpression(@"^\d{3}-\d-\d{6}-\d{3}$", ErrorMessage = "ISBN must be in the format '978-1-234567-910'.")]
        public required string Isbn { get; set; }

        /// <example>Test Title</example>
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public required string Title { get; set; }

        /// <example>Test Description</example>
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public required string Description { get; set; }

        /// <example>500</example>
        [Required(ErrorMessage = "List price is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "List price must be greater than or equal to 0.")]
        public required decimal ListPrice { get; set; }

        /// <example>79</example>
        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public required short Discount { get; set; }

        /// <example>2</example>
        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
        public required int Stock { get; set; }

        /// <example>2025-01-01</example>
        [Required(ErrorMessage = "Publication date is required.")]
        public required DateOnly PublicationDate { get; set; }

        /// <example>1</example>
        [Required(ErrorMessage = "AuthorId is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "AuthorId must be greater than or equal to 0.")]
        public required int AuthorId { get; set; }

        /// <example>1</example>
        [Required(ErrorMessage = "PublisherId is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "PublisherId must be greater than or equal to 0.")]
        public required int PublisherId { get; set; }

        /// <summary>
        /// 上傳書籍封面圖片（僅支援 .jpg/.png，最大 2MB）
        /// </summary>
        [Required(ErrorMessage = "UploadedImage is required.")]
        public required IFormFile UploadedImage { get; set; }

        // TODO: 抽離驗證邏輯
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 檢查圖片格式
            var formatErrors = ValidateImageFormat();
            foreach (var error in formatErrors)
            {
                yield return error;
            }

            // 檢查檔案大小
            var sizeErrors = ValidateImageSize();
            foreach (var error in sizeErrors)
            {
                yield return error;
            }
        }

        private IEnumerable<ValidationResult> ValidateImageFormat()
        {
            // 支援的 MIME 類型
            var allowedMimeTypes = new HashSet<string>
            {
                "image/jpeg",
                "image/png"
            };

            // 取得檔案的 MIME 類型
            var fileMimeType = UploadedImage.ContentType.ToLower();

            // 檢查檔案 MIME 類型是否在允許的範圍內
            if (!allowedMimeTypes.Contains(fileMimeType))
            {
                yield return new ValidationResult($"The file MIME type {fileMimeType} is not a valid image format.", [nameof(UploadedImage)]);
            }
        }

        private IEnumerable<ValidationResult> ValidateImageSize()
        {
            if (UploadedImage.Length > 2 * 1024 * 1024)  // 2MB
            {
                yield return new ValidationResult("The image size must not exceed 2MB.", [nameof(UploadedImage)]);
            }
        }
    }
}
