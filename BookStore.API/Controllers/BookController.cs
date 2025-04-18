using BookStore.DTO.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// 書藉相關
    /// </summary>
    [ApiController]
    public class BookController(IBookService bookService) : Controller
    {
        private readonly IBookService _bookService = bookService;

        /// <summary>
        /// 獲取書籍列表
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">登入成功</response>
        /// <response code="400">請求格式錯誤</response>
        [HttpGet("book")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryParameters bookQueryParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {
                return Ok(await _bookService.GetBooksAsync(bookQueryParameters));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
