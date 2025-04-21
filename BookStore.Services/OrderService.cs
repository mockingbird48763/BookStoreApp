using BookStore.Core.Exceptions;
using BookStore.Core.Extensions;
using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
using BookStore.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
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

        public async Task<PaginatedResult<OrderSummaryDto>> GetOrdersAsync(OrderQueryParameters orderQueryParameters)
        {
            var page = orderQueryParameters.Page;
            var pageSize = orderQueryParameters.PageSize;

            var query = _context.Orders
                .Include(o => o.Member)
                .FilterByOrderStatus(orderQueryParameters.OrderStatus)
                .FilterByPaymentStatus(orderQueryParameters.PaymentStatus)
                .FilterByPaymentMethod(orderQueryParameters.PaymentMethod)
                .FilterByShippingMethod(orderQueryParameters.ShippingMethod)
                .FilterByStartDate(orderQueryParameters.StartDate)
                .FilterByEndDate(orderQueryParameters.EndDate)
                .FilterByRoleAndMemberId(orderQueryParameters.RoleName, orderQueryParameters.MemberId);

            var totalCount = await query.CountAsync();

            // 查詢結果
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderSummaryDto()
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ShippingMethod = o.ShippingMethod,
                    MemberId = o.Member.Id,
                })
                .ToListAsync();

            return new PaginatedResult<OrderSummaryDto>()
            {
                Items = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDetailDto> GetOrderDetailAsync(OrderDetailRequest orderDetailRequest)
        {
            var id = orderDetailRequest.OrderId;
            var roleName = orderDetailRequest.RoleName;
            var memberId = orderDetailRequest.MemberId;

            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Include(o => o.Member)
                .Where(o => o.Id == id);

            // 如果是 Admin，則不需要過濾會員 ID
            if (roleName != RoleType.Admin.ToString())
            {
                query = query.Where(o => o.Member.Id == memberId);
            }

            var order = await query.FirstOrDefaultAsync()
                ?? throw new NotFoundException($"Order with ID {id} not found.");

            return ToOrderDetailDto(order);
        }

        private static OrderDetailDto ToOrderDetailDto(Order o)
        {
            return new OrderDetailDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                TotalPrice = o.TotalPrice,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingMethod = o.ShippingMethod,
                ShippingAddress = o.ShippingAddress,
                ShippingNote = o.ShippingNote ?? string.Empty,
                CreatedAt = o.CreatedAt,
                MemberId = o.Member.Id,
                MemberEmail = o.Member.Email,
                // .Select(oi => ToOrderDetailItemDto(oi))
                OrderDetailItems = [.. o.OrderItems.Select(ToOrderDetailItemDto)]
            };
        }

        private static OrderDetailItemDto ToOrderDetailItemDto(OrderItem oi)
        {
            return new OrderDetailItemDto
            {
                Id = oi.Id,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                BookId = oi.Book.Id,
                BookName = oi.Book.Title,
            };
        }
    }
}
