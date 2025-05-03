using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class OrderDetailRequest
    {
        public int OrderId { get; set; }
        public required String RoleName { get; set; }
        public int? MemberId { get; set; }
    }
}
