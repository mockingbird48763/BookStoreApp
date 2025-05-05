using Azure;
using Bogus;
using BookStore.Core.Exceptions;
using BookStore.Core.Extensions;
using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
using BookStore.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class OrdersService(ApplicationDbContext context, IUserInfoContext userInfoContext) : IOrdersService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IUserInfoContext _userInfoContext = userInfoContext;


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
                .FilterByEndDate(orderQueryParameters.EndDate);

            if (!(_userInfoContext.IsAdmin && orderQueryParameters.ViewAs == "admin"))
            {
                query = query.Where(o => o.Member.Id == int.Parse(_userInfoContext.UserId));
            }

            var totalCount = await query.CountAsync();

            // 查詢結果
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
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

        public async Task<int> CreateOrderAsync(CreateOrderRequest createOrderRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var bookIds = createOrderRequest.OrderItems.Select(x => x.BookId).ToList();

            // Dictionary<int, Book>
            var books = await _context.Books
                .Where(b => bookIds.Contains(b.Id))
                .ToDictionaryAsync(b => b.Id);

            // 驗證庫存是否足夠
            foreach (var item in createOrderRequest.OrderItems)
            {
                if (!books.TryGetValue(item.BookId, out Book? value))
                    throw new NotFoundException($"Book not found: {item.BookId}");

                if (value.Stock < item.Quantity)
                    throw new InsufficientStockException($"Insufficient stock for book: {item.BookId}");
            }

            var memberId = createOrderRequest.MemberId;
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == memberId)
                ?? throw new NotFoundException($"Member with ID {memberId} not found.");

            var orderItems = createOrderRequest.OrderItems
                .Select(item =>
                {
                    var book = books[item.BookId];
                    // 整數除法的截斷，結果是 0，不是 0.2
                    // int result = discount / 100;
                    var unitPrice = Math.Round(book.ListPrice * (book.Discount / 100m), 0);

                    return new OrderItem
                    {
                        Book = book,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice
                    };
                }).ToList();

            // 建立訂單
            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                OrderNumber = GenerateOrderNumber(DateTime.UtcNow),
                Member = member,
                ShippingAddress = createOrderRequest.ShippingAddress,
                TotalPrice = orderItems.Sum(item => item.Quantity * item.UnitPrice),
                PaymentMethod = createOrderRequest.PaymentMethod,
                ShippingMethod = createOrderRequest.ShippingMethod,
                ShippingNote = createOrderRequest.ShippingNote ?? string.Empty,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);

            // 扣除庫存
            foreach (var item in createOrderRequest.OrderItems)
            {
                books[item.BookId].Stock -= item.Quantity;
            }

            try
            {
                await _context.SaveChangesAsync(); // 此時會自動檢查 RowVersion 是否有改變
                await transaction.CommitAsync();
                return order.Id;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InsufficientStockException("訂單失敗，書籍庫存已變更，請重新下單。");
            }
        }

        public async Task<FileStream> GenerateOrderReportAsync(OrderQueryParameters orderQueryParameters)
        {
            var filePath = Path.Combine(Path.GetTempPath(), "order-report.csv");

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8, 1024))
            {
                writer.WriteLine("訂單編號,顧客名稱,訂單狀態,訂單日期,總金額"); // CSV 標題行

                var batchSize = 1000;
                var skip = 0;
                var orders = await _context.Orders
                    .Include(o => o.Member)
                    .FilterByStartDate(orderQueryParameters.StartDate)
                    .FilterByEndDate(orderQueryParameters.EndDate)
                    .Skip(skip)
                    .Take(batchSize)
                    .ToListAsync();

                decimal totalAmount = 0;

                while (orders.Count != 0)
                {
                    foreach (var order in orders)
                    {
                        writer.WriteLine($"{order.OrderNumber},{order.Member.Email},{order.OrderStatus},{DateOnly.FromDateTime(order.CreatedAt)},{order.TotalPrice}");
                        totalAmount += order.TotalPrice;
                    }

                    writer.Flush();

                    skip += batchSize;
                    orders = await _context.Orders
                        .Skip(skip)
                        .Take(batchSize)
                        .ToListAsync();
                }

                writer.WriteLine($"Total,{totalAmount:F2}");
                writer.Flush();
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public async Task UpdateOrderAsync(int id, UpdateOrderRequest updateOrderRequest)
        {
            var order = await _context.Orders.FindAsync(id) ?? throw new NotFoundException($"Order with ID {id} was not found.");

            if (updateOrderRequest.OrderStatus.HasValue)
                order.OrderStatus = updateOrderRequest.OrderStatus.Value;

            if (updateOrderRequest.PaymentStatus.HasValue)
                order.PaymentStatus = updateOrderRequest.PaymentStatus.Value;

            await _context.SaveChangesAsync();
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

        private static string GenerateOrderNumber(DateTime date)
        {
            var dataFormat = date.ToString("yyyyMMdd_HHmmss");
            var orderNumber = $"{dataFormat}_{GenerateRandomCode()}";
            return orderNumber;
        }

        private static string GenerateRandomCode(int length = 4)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string([.. Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)])]);
        }
    }
}
