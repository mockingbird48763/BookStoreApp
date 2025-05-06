using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "OrderNubmer is required.")]
        [StringLength(20, MinimumLength = 20, ErrorMessage = "OrderNubmer must be exactly 20 characters.")]
        [RegularExpression(@"^\d{8}_\d{6}_\d{4}_[a-z0-9]{4}$", ErrorMessage = "OrderNubmer must be in the format '20000000_000000_0000'.")]
        public required string OrderNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total Price must be greater than 1.")]
        public required Decimal TotalPrice { get; set; }

        [StringLength(150, ErrorMessage = "ShippingAddress cannot be longer than 150 characters.")]
        public required String ShippingAddress { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public required PaymentMethod PaymentMethod { get; set; }

        public required ShippingMethod ShippingMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(255, ErrorMessage = "ShippingMethod cannot be longer than 255 characters.")]
        public String ShippingNote { get; set; } = String.Empty;

        public required Member Member { get; set; }

        public required ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
