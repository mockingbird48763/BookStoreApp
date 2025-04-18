using BookStore.DTO.Request;
using BookStore.DTO.Response;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface IBookService
    {
        public Task<PaginatedResult<BookSummaryDto>> GetBooksAsync(BookQueryParameters bookQueryParameters);
    }
}
