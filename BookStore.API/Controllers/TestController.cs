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


        // ���_���|���t�m
        // $env:GOOGLE_APPLICATION_CREDENTIALS="D:\secret.json"
        // echo $env:GOOGLE_APPLICATION_CREDENTIALS

        // export GOOGLE_APPLICATION_CREDENTIALS="/home/user/secret.json"

        /// <summary>
        /// ���� Google Cloud Storage �U���ɮ�
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
                    // �ɮק䤣�쪺���p
                    return NotFound("File not found");
                }

                // ���m�y����m��0�A�o�˱q�Y�}�lŪ��
                stream.Position = 0;

                return File(stream, obj.ContentType, obj.Name);
            }
            catch (GoogleApiException apiEx)
            {
                // �p�G�O Google Cloud Storage API �����~�A�i�H�B�z��
                return StatusCode(500, $"Google API Error: {apiEx.Message}");
            }
            catch (Exception ex)
            {
                // ������L���������~
                return StatusCode(500, $"Unexpected Error: {ex.Message}");
            }
        }

        /// <summary>
        /// ���� Google Cloud Storage �W���ɮ�
        /// </summary>
        [HttpPost("upload-file")]
        public async Task<IActionResult> AddFile([FromBody] FileUpload fileUpload) { 
            var client = StorageClient.Create();
            var file = Encoding.UTF8.GetBytes(fileUpload.File);

            var obj = await client.UploadObjectAsync("book_store_storage_mockingbird48763_v2", fileUpload.Name, fileUpload.Type, new MemoryStream(file));
            return Ok();
        }

        /// <summary>
        /// ���� Google Cloud Storage ñ�W URL
        /// </summary>
        [HttpGet("signed-url")]
        public async Task<IActionResult> GenerateSignedUrl(string fileName)
        {
            UrlSigner urlSigner = UrlSigner.FromCredentialFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            // ñ�W�i�H�]�m�֨��A��֪���
            string signedUrl = await urlSigner.SignAsync(
                bucket: "book_store_storage_mockingbird48763_v2",
                objectName: fileName,
                duration: TimeSpan.FromHours(1) // �]�wñ�W URL �����Įɶ�
            );

            // �p�G�A���Q��^ URL�A�ӬO���e����(�奻�ιϤ�(�� byte[]))�A�A���Q���S URL �ɨϥ�(�����S��URL�A����H���i�H�X�ݡA��ñ�W�N�S���N�q)
            // HttpClient httpClient = new HttpClient(); // ���ϥΪ`�J���覡�Ы� HttpClient
            // HttpResponseMessage response = await httpClient.GetAsync(signedUrl);
            // string content = await response.Content.ReadAsStringAsync();
            return Ok(new { signedUrl });
        }

        public class FileUpload
        {
            public required string Name { get; set; }
            public required string Type { get; set; }

            // ��r�ɮת����e
            public required string File { get; set; }
        }
    }
}
