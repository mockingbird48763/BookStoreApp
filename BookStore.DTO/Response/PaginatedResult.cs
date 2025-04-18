using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }           // 全部筆數
        public int Page { get; set; }                 // 現在是第幾頁
        public int PageSize { get; set; }             // 每頁幾筆
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // 總頁數
    }
}
