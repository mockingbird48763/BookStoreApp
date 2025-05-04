using BookStore.DTO.Request;
using BookStore.Models.Enums;
using Google;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

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

        [HttpPost("download-report1")]
        [AllowAnonymous]
        public IActionResult GenerateReportA()
        {
            string data = "ID,Name,Amount\n1,John Doe,100\n2,Jane Smith,150\n3,Bob Johnson,200";
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(data);

            string fileName = "report.csv";
            return File(fileBytes, "text/csv", fileName);  // 使用 File 方法直接返回檔案
        }

        [HttpPost("download-report2")]
        [AllowAnonymous]
        public IActionResult GenerateReportB()
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: false))
            {
                // 固定的常量報表資料
                string[] reportData = new string[]
                {
                    "OrderID,CustomerName,Amount",
                    "1,John Doe,100.00",
                    "2,Jane Smith,200.50",
                    "3,Jim Brown,150.75"
                };

                // 寫入資料到 CSV 格式
                foreach (var line in reportData)
                {
                    writer.WriteLine(line);
                }

                writer.Flush();
            }

            // memoryStream.Position = 0; // 重置 MemoryStream 的位置以便讀取，配合 leaveOpen: false
            Response.Headers.Append("Content-Disposition", "attachment; filename=report.csv");
            return new FileStreamResult(memoryStream, "text/csv");
        }


        // 金鑰路徑的配置
        // $env:GOOGLE_APPLICATION_CREDENTIALS="D:\secret.json"
        // echo $env:GOOGLE_APPLICATION_CREDENTIALS

        // export GOOGLE_APPLICATION_CREDENTIALS="/home/user/secret.json"

        /// <summary>
        /// 測試 Google Cloud Storage 下載檔案
        /// </summary>
        [HttpGet("get-file")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            var client = StorageClient.Create();
            var stream = new MemoryStream();
            try
            {
                var obj = await client.DownloadObjectAsync("book_store_storage_mockingbird48763_v2", fileName, stream);

                if (obj == null)
                {
                    // 檔案找不到的情況
                    return NotFound("File not found");
                }

                // 重置流的位置為0，這樣從頭開始讀取
                stream.Position = 0;

                return File(stream, obj.ContentType, obj.Name);
            }
            catch (GoogleApiException apiEx)
            {
                // 如果是 Google Cloud Storage API 的錯誤，可以處理它
                return StatusCode(500, $"Google API Error: {apiEx.Message}");
            }
            catch (Exception ex)
            {
                // 捕捉其他類型的錯誤
                return StatusCode(500, $"Unexpected Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 測試 Google Cloud Storage 上傳檔案
        /// </summary>
        [HttpPost("upload-file")]
        public async Task<IActionResult> AddFile([FromBody] FileUpload fileUpload) { 
            var client = StorageClient.Create();
            var file = Encoding.UTF8.GetBytes(fileUpload.File);

            var obj = await client.UploadObjectAsync("book_store_storage_mockingbird48763_v2", fileUpload.Name, fileUpload.Type, new MemoryStream(file));
            return Ok();
        }

        /// <summary>
        /// 測試 Google Cloud Storage 簽名 URL
        /// </summary>
        [HttpGet("signed-url")]
        public async Task<IActionResult> GenerateSignedUrl(string fileName)
        {
            UrlSigner urlSigner = UrlSigner.FromCredentialFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            // 簽名可以設置快取，減少阻塞
            string signedUrl = await urlSigner.SignAsync(
                bucket: "book_store_storage_mockingbird48763_v2",
                objectName: fileName,
                duration: TimeSpan.FromHours(1) // 設定簽名 URL 的有效時間
            );

            // 如果你不想返回 URL，而是內容本身(文本或圖片(用 byte[]))，你不想暴露 URL 時使用(但暴露此URL，任何人都可以訪問，那簽名就沒有意義)
            // HttpClient httpClient = new HttpClient(); // 應使用注入的方式創建 HttpClient
            // HttpResponseMessage response = await httpClient.GetAsync(signedUrl);
            // string content = await response.Content.ReadAsStringAsync();
            return Ok(new { signedUrl });
        }

        public class FileUpload
        {
            public required string Name { get; set; }
            public required string Type { get; set; }

            // 文字檔案的內容
            public required string File { get; set; }
        }
    }
}
