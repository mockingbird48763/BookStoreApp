using BookStore.Core.Extensions;
using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
using BookStore.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class OrderService(ApplicationDbContext context) : IOrderService
    {

        private readonly ApplicationDbContext _context = context;

        public async Task<PaginatedResult<AdminOrderSummaryDto>> GetAllOrdersAsync(OrderQueryParameters orderQueryParameters)
        {
            var page = orderQueryParameters.Page;
            var pageSize = orderQueryParameters.PageSize;

            var query = _context.Orders
                .FilterByOrderStatus(orderQueryParameters.OrderStatus)
                .FilterByPaymentStatus(orderQueryParameters.PaymentStatus)
                .FilterByPaymentMethod(orderQueryParameters.PaymentMethod)
                .FilterByShippingMethod(orderQueryParameters.ShippingMethod)
                .FilterByStartDate(orderQueryParameters.StartDate)
                .FilterByEndDate(orderQueryParameters.EndDate)
                .FilterByMember(orderQueryParameters.MemberId);

            var totalCount = await query.CountAsync();

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AdminOrderSummaryDto()
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNubmer,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ShippingMethod = o.ShippingMethod,
                })
                .ToListAsync();

            return new PaginatedResult<AdminOrderSummaryDto>()
            {
                Items = [.. orders],
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public Task<PaginatedResult<Order>> GetOrdersByMemberIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
