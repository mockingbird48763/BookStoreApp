using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
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

        /// <summary>
        /// 獲取書籍詳細資料
        /// </summary>
        /// <param name="id">書籍的唯一識別碼</param>
        /// <returns>書籍資料或錯誤訊息</returns>
        /// <response code="200">成功，回傳書籍資料</response>
        /// <response code="400">請求格式錯誤，例如 ID 無效</response>
        /// <response code="404">找不到指定的書籍</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            return Ok(book);
        }

        // TODO: 添加權限
        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="request">書籍資料</param>
        /// <returns>新增成功的書籍</returns>
        /// <response code="201">新增成功</response>
        /// <response code="400">資料格式錯誤</response>
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookRequest request)
        {
            var bookId = await _bookService.CreateBookAsync(request);
            return StatusCode(201, new { id = bookId });
        }

        // TODO: 添加權限
        /// <summary>
        /// 修改書籍詳細資料
        /// </summary>
        /// <param name="id">書籍的唯一識別碼</param>
        /// <param name="updateRequest">書籍資料</param>
        /// <returns></returns>
        /// <response code="204">修改成功</response>
        /// <response code="400">資料格式錯誤</response>
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromForm] UpdateBookRequest updateRequest)
        {
            await _bookService.UpdateBookAsync(id, updateRequest);
            return NoContent();
        }
    }
}
