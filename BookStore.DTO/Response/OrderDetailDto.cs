using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class OrderDetailDto
    {
        public int Id { get; set; }

        public required string OrderNumber { get; set; }

        public required decimal TotalPrice { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public ShippingMethod ShippingMethod { get; set; }

        public required string ShippingAddress { get; set; }

        public string ShippingNote { get; set; } = String.Empty;

        public required DateTime CreatedAt { get; set; }

        public required int MemberId { get; set; }

        public required string MemberEmail { get; set; }

        public required List<OrderDetailItemDto> OrderDetailItems { get; set; }
    }
}
