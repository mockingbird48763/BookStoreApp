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
    public interface IBooksService
    {
        Task<int> CreateBookAsync(CreateBookRequest request);
        Task<PaginatedResult<BookSummaryDto>> GetBooksAsync(BookQueryParameters bookQueryParameters);
        Task<BookDetailDto> GetByIdAsync(int id);
        Task UpdateBookAsync(int id, UpdateBookRequest updateRequest);
        Task UpdateBooksVisibilityAsync(List<BookVisibilityUpdateRequest> requests);
        Task<List<CartBookItemDto>> GetBooksForCartAsync(List<int> ids);
    }
}
