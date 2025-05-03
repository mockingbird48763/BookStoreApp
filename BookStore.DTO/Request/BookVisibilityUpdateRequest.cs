using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    /// <summary>
    /// 書籍可見性更新請求
    /// </summary>
    public class BookVisibilityUpdateRequest
    {
        /// <summary>
        /// 書籍的 ID
        /// </summary>
        /// <example>1</example>
        public int BookId { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        /// <example>true</example>
        public bool IsVisible { get; set; }
    }
}
