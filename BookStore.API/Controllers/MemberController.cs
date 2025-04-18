﻿using BookStore.DTO.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// 會員相關
    /// </summary>
    [ApiController]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                await _memberService.RegisterAsync(registerMemberRequest);
                return Ok(new { message = "註冊成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
