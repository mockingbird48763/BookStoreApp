using BookStore.Core.Exceptions;
using BookStore.Models;
using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Extensions
{
    public static class OrderQueryExtensions
    {
        public static IQueryable<Order> FilterByMember(this IQueryable<Order> query, int? memberId)
        {
            if (memberId.HasValue)
            {
                query = query.Where(o => o.Member.Id == memberId.Value);
            }
            return query;
        }

        public static IQueryable<Order> FilterByOrderStatus(this IQueryable<Order> query, OrderStatus? orderStatus)
        {
            if (orderStatus.HasValue)
            {
                query = query.Where(o => o.OrderStatus == orderStatus.Value);
            }
            return query;
        }

        public static IQueryable<Order> FilterByPaymentStatus(this IQueryable<Order> query, PaymentStatus? paymentStatus)
        {
            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
            }
            return query;
        }

        public static IQueryable<Order> FilterByPaymentMethod(this IQueryable<Order> query, PaymentMethod? paymentStatus)
        {
            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentMethod == paymentStatus.Value);
            }
            return query;
        }

        public static IQueryable<Order> FilterByShippingMethod(this IQueryable<Order> query, ShippingMethod? shippingMethod)
        {
            if (shippingMethod.HasValue)
            {
                query = query.Where(o => o.ShippingMethod == shippingMethod.Value);
            }
            return query;
        }

        public static IQueryable<Order> FilterByStartDate(this IQueryable<Order> query, DateOnly? startDate)
        {
            if (startDate.HasValue)
            {
                // 轉成當天 00:00:00
                var start = startDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(o => o.CreatedAt >= start);
            }
            return query;
        }

        public static IQueryable<Order> FilterByEndDate(this IQueryable<Order> query, DateOnly? endDate)
        {
            if (endDate.HasValue)
            {
                // 轉成當天 23:59:59.9999999
                var end = endDate.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(b => b.CreatedAt <= end);
            }
            return query;
        }
    }
}
