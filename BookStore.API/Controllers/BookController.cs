using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers
{
    /// <summary>
    /// 書藉相關
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BookController(IBookService bookService) : Controller
    {
        private readonly IBookService _bookService = bookService;

        /// <summary>
        /// 獲取書籍列表
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">登入成功</response>
        /// <response code="400">請求格式錯誤</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryParameters bookQueryParameters)
        {
            return Ok(await _bookService.GetBooksAsync(bookQueryParameters));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetBookById([FromRoute] int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            return Ok(book);
        }
    }
}
