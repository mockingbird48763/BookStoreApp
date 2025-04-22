using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface IOrderService
    {
        Task<PaginatedResult<OrderSummaryDto>> GetOrdersAsync(OrderQueryParameters orderQueryParameters);

        Task<OrderDetailDto> GetOrderDetailAsync(OrderDetailRequest orderDetailRequest);

        Task<int> CreateOrderAsync(CreateOrderRequest createOrderRequest);
    }
}
