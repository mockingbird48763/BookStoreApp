using BookStore.DTO.Request;
using BookStore.Models.Enums;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        public int OrderId { get; private set; }

        // TODO: 權限設定
        /// <summary>
        /// 獲取訂單列表
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">請求成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQueryParameters orderQueryParameters)
        {
            var isAdmin = User.IsInRole(RoleType.Admin.ToString());
            orderQueryParameters.RoleName = isAdmin ? RoleType.Admin.ToString() : RoleType.User.ToString();

            if (!isAdmin)
            {
                orderQueryParameters.MemberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            }

            return Ok(await _orderService.GetOrdersAsync(orderQueryParameters));
        }

        /// <summary>
        /// 獲取訂單詳細內容
        /// </summary>
        /// <returns>請求成功或失敗的訊息</returns>
        /// <response code="200">請求成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetOrderDetail([FromRoute] int id)
        {
            var isAdmin = User.IsInRole(RoleType.Admin.ToString());

            var orderDetailRequest = new OrderDetailRequest
            {
                OrderId = id,
                RoleName = isAdmin ? RoleType.Admin.ToString() : RoleType.User.ToString(),
                MemberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            };
            
            return Ok(await _orderService.GetOrderDetailAsync(orderDetailRequest));
        }
    }
}
