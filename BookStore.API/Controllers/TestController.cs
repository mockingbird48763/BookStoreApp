using BookStore.DTO.Request;
using BookStore.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
