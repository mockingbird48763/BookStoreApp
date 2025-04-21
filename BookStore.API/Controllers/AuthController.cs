using BookStore.DTO.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// 權限相關
    /// </summary>
    [ApiController]
    public class AuthController(IAuthService authService) : Controller
    {

        private readonly IAuthService _authService = authService;

        // TODO: Register => Login
        /// <summary>
        /// 登錄會員
        /// </summary>
        /// <param name="loginRequest">登入所需的會員資料</param>
        /// <returns>登入成功或失敗的訊息</returns>
        /// <response code="200">登入成功</response>
        /// <response code="400">請求格式錯誤或驗證失敗</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {
                string token = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
