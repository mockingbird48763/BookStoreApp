using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class UpdateBookRequest
    {
        /// <example>978-1-234567-911</example>
        [StringLength(16, MinimumLength = 16, ErrorMessage = "ISBN must be exactly 16 characters.")]
        [RegularExpression(@"^\d{3}-\d-\d{6}-\d{3}$", ErrorMessage = "ISBN must be in the format '978-1-234567-910'.")]
        public string? Isbn { get; set; }

        /// <example>改善程式效能</example>
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string? Title { get; set; }

        /// <example>這本書聚焦於提高程式效能，介紹如何優化代碼，改進算法，減少計算時間與資源消耗，並探討常見的效能瓶頸問題。</example>
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string? Description { get; set; }

        /// <example>550</example>
        [Range(0, int.MaxValue, ErrorMessage = "List price must be greater than or equal to 0.")]
        public decimal? ListPrice { get; set; }

        /// <example>15</example>
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public short? Discount { get; set; }

        /// <example>1</example>
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
        public int? Stock { get; set; }

        /// <example>2021-04-30</example>
        public DateOnly? PublicationDate { get; set; }

        /// <example>1</example>
        [Range(0, int.MaxValue, ErrorMessage = "AuthorId must be greater than or equal to 0.")]
        public int? AuthorId { get; set; }

        /// <example>1</example>
        [Range(0, int.MaxValue, ErrorMessage = "PublisherId must be greater than or equal to 0.")]
        public int? PublisherId { get; set; }

        /// <summary>
        /// 上傳書籍封面圖片（僅支援 .jpg/.png，最大 2MB）
        /// </summary>
        public IFormFile? UploadedImage { get; set; }
    }
}
