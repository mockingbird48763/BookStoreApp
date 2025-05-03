using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class UpdateOrderRequest
    {
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus? OrderStatus { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
