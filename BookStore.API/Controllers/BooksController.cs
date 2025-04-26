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
    public class BooksController(IBooksService bookService) : Controller
    {
        private readonly IBooksService _bookService = bookService;

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

        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="request">書籍資料</param>
        /// <returns>新增成功的書籍</returns>
        /// <response code="201">新增成功</response>
        /// <response code="400">資料格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookRequest request)
        {
            var bookId = await _bookService.CreateBookAsync(request);
            return StatusCode(201, new { id = bookId });
        }

        /// <summary>
        /// 修改書籍詳細資料
        /// </summary>
        /// <param name="id">書籍的唯一識別碼</param>
        /// <param name="updateRequest">書籍資料，multipart/form-data 在 Swagger 存在限制，無法傳送 null</param>
        /// <response code="204">修改成功</response>
        /// <response code="400">資料格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        /// <response code="404">找不到指定的書籍</response>
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromForm] UpdateBookRequest updateRequest)
        {
            await _bookService.UpdateBookAsync(id, updateRequest);
            return NoContent();
        }

        /// <summary>
        /// 修改書籍的可見屬性
        /// </summary>
        /// <response code="204">修改成功</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpPatch]
        [Route("visibility")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBooksVisibility(List<BookVisibilityUpdateRequest> requests) {
            await _bookService.UpdateBooksVisibilityAsync(requests);
            return NoContent();
        }

        /// <summary>
        /// 批量獲取書籍資料，用於購物車頁面
        /// </summary>
        /// <response code="200">獲取成功</response>
        /// <response code="400">資料格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpGet("cart")]
        [Authorize(Roles = "User, Admin")]

        public async Task<IActionResult> GetBooksForCartByIds([FromQuery] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("IDs cannot be null or empty.");

            var result = await _bookService.GetBooksForCartAsync(ids);

            if (result.Count != 0)
                return Ok(result);

            return NotFound("No books available.");
        }
    }
}
