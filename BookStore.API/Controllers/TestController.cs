using BookStore.DTO.Request;
using BookStore.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// �o�O�@�Ӵ��ժ����
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Guest �v������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<string> ForGuest()
        {
            return "Hello, Guest";
        }

        /// <summary>
        /// User �v������
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
        /// Admin �v������
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
        /// �������⪺�n�J�Τ��v������
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

        // �]�w�ɮ��x�s���|
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        /// <summary>
        /// ���a�Ϥ��W�Ǵ���
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload-image")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("�ФW�ǹϤ�");

            var now = DateTime.Now;
            string datePart = now.ToString("yyyyMMdd_HHmmss");

            // var randomStr = Path.GetRandomFileName().Substring(0, 4).ToUpper();
            var randomStr = Path.GetRandomFileName()[..4].ToUpper(); // �ͦ��H���|�Ӧr���M�Ʀr���զX

            // �����q FileName �ݩʨ��o���ɦW
            var fileName = image.FileName;
            var fileExtension = Path.GetExtension(fileName); // �Ҧp ".jpg", ".png"

            // �c���ɮ��x�s���|�A�o�̷|�x�s�b BookStore.API/wwwroot/images �ؿ��U
            var filePath = Path.Combine(_uploadsFolder, $"{datePart}_{randomStr}{fileExtension}");

            // �T�O��Ƨ��s�b�A�p�G���s�b�h�Ы�
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            // �ϥ� FileStream �x�s�ɮ�
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream); // �N�W�Ǫ��ɮ׼g�J���w���|
            }

            return Ok(new { message = "�W�Ǧ��\", fileName = $"{datePart}_{randomStr}{fileExtension}" });
        }

        [HttpPost("download-report1")]
        [AllowAnonymous]
        public IActionResult GenerateReportA()
        {
            string data = "ID,Name,Amount\n1,John Doe,100\n2,Jane Smith,150\n3,Bob Johnson,200";
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(data);

            string fileName = "report.csv";
            return File(fileBytes, "text/csv", fileName);  // �ϥ� File ��k������^�ɮ�
        }

        [HttpPost("download-report2")]
        [AllowAnonymous]
        public IActionResult GenerateReportB()
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: false))
            {
                // �T�w���`�q������
                string[] reportData = new string[]
                {
                    "OrderID,CustomerName,Amount",
                    "1,John Doe,100.00",
                    "2,Jane Smith,200.50",
                    "3,Jim Brown,150.75"
                };

                // �g�J��ƨ� CSV �榡
                foreach (var line in reportData)
                {
                    writer.WriteLine(line);
                }

                writer.Flush();
            }

            // memoryStream.Position = 0; // ���m MemoryStream ����m�H�KŪ���A�t�X leaveOpen: false
            Response.Headers.Append("Content-Disposition", "attachment; filename=report.csv");
            return new FileStreamResult(memoryStream, "text/csv");
        }
    }
}
