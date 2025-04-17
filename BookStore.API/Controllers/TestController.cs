using BookStore.DTO.Request;
using Microsoft.AspNetCore.Mvc;

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
        /// �o�O�@�� GET API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hello";
        }

        /// <summary>
        /// �o�O�@�Ӱݦn�� API
        /// </summary>
        /// <returns></returns>
        [HttpPost("hello")]
        public ActionResult<string> Hello([FromBody] TestRequest request)
        {
            return "Hello, " + request.Name;
        }
    }
}
