﻿using BookStore.Models.Enums;
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
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus? OrderStatus { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus? PaymentStatus { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod? PaymentMethod { get; set; }

        [EnumDataType(typeof(ShippingMethod))]
        public ShippingMethod? ShippingMethod { get; set; }

        public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(new DateTime(2000, 1, 1));

        public DateOnly? EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        /// <summary>
        /// 檢視模式，根據角色切換不同資料顯示。
        /// </summary>
        /// <remarks>
        /// 可用值：
        /// <list type="bullet">
        ///   <item><description><c>admin</c> - 管理者視角，包含所有資料。</description></item>
        /// </list>
        /// </remarks>
        /// <example>admin</example>
        public String? ViewAs { get; set; } = String.Empty;
    }
}
