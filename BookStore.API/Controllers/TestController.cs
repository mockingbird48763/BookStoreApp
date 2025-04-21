using BookStore.DTO.Request;
using BookStore.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// 這是一個測試的控制器
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Guest 權限測試
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<string> ForGuest()
        {
            return "Hello, Guest";
        }

        /// <summary>
        /// User 權限測試
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public IActionResult ForUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { userId, email, role });
        }

        /// <summary>
        /// Admin 權限測試
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult ForAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { userId, email, role });
        }

        /// <summary>
        /// 不限角色的登入用戶權限測試
        /// </summary>
        /// <returns></returns>
        [HttpGet("authenticatedUsers")]
        [Authorize]
        public IActionResult ForAuthenticatedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            User.IsInRole("Admin");
            User.IsInRole("Manager");
            return Ok(new { 
                userId, 
                email, 
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(), // ["Admin", "User"]
                isAdmin = User.IsInRole(RoleType.Admin.ToString()),
                isUser = User.IsInRole(RoleType.User.ToString()),
            });
        }

        // 設定檔案儲存路徑
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        /// <summary>
        /// 本地圖片上傳測試
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload-image")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("請上傳圖片");

            var now = DateTime.Now;
            string datePart = now.ToString("yyyyMMdd_HHmmss");

            // var randomStr = Path.GetRandomFileName().Substring(0, 4).ToUpper();
            var randomStr = Path.GetRandomFileName()[..4].ToUpper(); // 生成隨機四個字母和數字的組合

            // 直接從 FileName 屬性取得副檔名
            var fileName = image.FileName;
            var fileExtension = Path.GetExtension(fileName); // 例如 ".jpg", ".png"

            // 構建檔案儲存路徑，這裡會儲存在 BookStore.API/wwwroot/images 目錄下
            var filePath = Path.Combine(_uploadsFolder, $"{datePart}_{randomStr}{fileExtension}");

            // 確保資料夾存在，如果不存在則創建
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            // 使用 FileStream 儲存檔案
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream); // 將上傳的檔案寫入指定路徑
            }

            return Ok(new { message = "上傳成功", fileName = $"{datePart}_{randomStr}{fileExtension}" });
        }
    }
}
