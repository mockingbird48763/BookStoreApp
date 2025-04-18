using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class LoginRequest
    {
        /// <summary>
        /// 使用者 Email
        /// </summary>
        /// <example>test@example.com</example>
        [Required(ErrorMessage = "Email 是必填的")]
        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        [StringLength(256, ErrorMessage = "Email 不能超過 256 字元")]
        public required string Email { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        /// <example>abcd1234</example>
        [Required(ErrorMessage = "密碼是必填的")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度必須至少 6~20 字元")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$",
            ErrorMessage = "密碼需為8~20字元，且包含英文字母與數字")]
        public required string Password { get; set; }
    }
}
