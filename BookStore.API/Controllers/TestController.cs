using BookStore.DTO.Request;
using Microsoft.AspNetCore.Mvc;

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
        /// 這是一個 GET API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hello";
        }

        /// <summary>
        /// 這是一個問好的 API
        /// </summary>
        /// <returns></returns>
        [HttpPost("hello")]
        public ActionResult<string> Hello([FromBody] TestRequest request)
        {
            return "Hello, " + request.Name;
        }
    }
}
