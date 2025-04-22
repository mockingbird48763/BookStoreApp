using BookStore.DTO.Request;
using BookStore.Models.Enums;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        /// <summary>
        /// 創建訂單
        /// </summary>
        /// <returns>創建成功或失敗的訊息</returns>
        /// <response code="201">創建成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        /// <response code="404">查無此商品</response>
        /// <response code="409">庫存不足</response>
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest createOrderRequest) {
            createOrderRequest.MemberId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var id = await _orderService.CreateOrderAsync(createOrderRequest);

            return StatusCode(201, new { Id = id });
        }

        /// <summary>
        /// 修改訂單狀態
        /// </summary>
        /// <returns>修改成功或失敗的訊息</returns>
        /// <response code="204">修改成功</response>
        /// <response code="400">請求格式錯誤</response>
        /// <response code="401">未經身份驗證</response>
        /// <response code="403">授權不足</response>
        /// <response code="404">查無此訂單</response>
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrder([FromRoute] int id, [FromBody] UpdateOrderRequest updateOrderRequest)
        {
            await _orderService.UpdateOrderAsync(id, updateOrderRequest);
            return NoContent();
        }
    }
}
