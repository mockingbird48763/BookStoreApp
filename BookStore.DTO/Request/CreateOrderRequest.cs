using BookStore.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class CreateOrderRequest
    {
        [SwaggerIgnore]
        public int MemberId { get; set; }

        /// <example>
        /// [{ "BookId": 1, "Quantity": 3 }, { "BookId": 2, "Quantity": 1 }, { "BookId": 3, "Quantity": 3 }]
        /// </example>
        public List<CreateOrderItem> OrderItems { get; set; } = [];

        /// <example>桃園市中正區松山路666號</example>
        public required string ShippingAddress { get; set; }

        /// <example>1</example>
        public PaymentMethod PaymentMethod { get; set; }

        /// <example>1</example>
        public ShippingMethod ShippingMethod { get; set; }

        /// <example>請小心輕放</example>
        public string? ShippingNote { get; set; } = string.Empty;
    }
}
