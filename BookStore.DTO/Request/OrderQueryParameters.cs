using BookStore.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class OrderQueryParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0.")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "MemberId must be greater than 0.")]
        public int? MemberId { get; set; }

        // 合法參數
        // ?OrderStatus=0
        // ?OrderStatus=Pending (無視大小寫)
        // 枚舉內建攔截機制
        public OrderStatus? OrderStatus { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public ShippingMethod? ShippingMethod { get; set; }

        public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(new DateTime(2000, 1, 1));

        public DateOnly? EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        [SwaggerIgnore]
        public String RoleName { get; set; } = String.Empty;

    }
}
