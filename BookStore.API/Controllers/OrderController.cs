using BookStore.DTO.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    [ApiController]
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        // TODO: 權限設定
        /// <summary>
        /// 獲取全部訂單列表
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">請求成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpGet]
        [Route("api/admin/[controller]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersForAdmin([FromQuery] OrderQueryParameters orderQueryParameters)
        {
            return Ok(await _orderService.GetAllOrdersAsync(orderQueryParameters));
        }

        /// <summary>
        /// 獲取會員自身的訂單列表
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">請求成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpGet]
        [Route("api/[controller]")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetOrdersForMember([FromQuery] OrderQueryParameters orderQueryParameters)
        {
            orderQueryParameters.MemberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _orderService.GetAllOrdersAsync(orderQueryParameters));
        }
    }
}
