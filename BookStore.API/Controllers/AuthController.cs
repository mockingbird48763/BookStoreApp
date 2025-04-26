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
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, IMembersService memberService) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IMembersService _memberService = memberService;

        /// <summary>
        /// 登錄會員
        /// </summary>
        /// <param name="loginRequest">登入所需的會員資料</param>
        /// <returns>登入成功或失敗的訊息</returns>
        /// <response code="200">登入成功</response>
        /// <response code="400">請求格式錯誤或驗證失敗</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            string token = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);
            return Ok(new { token });
        }

        /// <summary>
        /// 註冊新會員
        /// </summary>
        /// <param name="registerMemberRequest">註冊所需的會員資料</param>
        /// <returns>註冊成功或錯誤的訊息</returns>
        /// <response code="200">註冊成功</response>
        /// <response code="400">請求格式錯誤或驗證失敗</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterMemberRequest registerMemberRequest)
        {
            await _memberService.RegisterAsync(registerMemberRequest);
            return Ok(new { message = "註冊成功" });
        }
    }
}
