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
    }
}
