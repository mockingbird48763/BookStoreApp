using BookStore.Core.Settings;
using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Core.Extensions;
using BookStore.Core.Exceptions;
using BookStore.Models;

namespace BookStore.Services
{
    public class BookService(ApplicationDbContext context, IImageStorageService imageStorageService) : IBookService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IImageStorageService _imageStorageService = imageStorageService;

        public async Task<PaginatedResult<BookSummaryDto>> GetBooksAsync(BookQueryParameters bookQueryParameters)
        {
            var page = bookQueryParameters.Page;
            var pageSize = bookQueryParameters.PageSize;

            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .AsQueryable();

            query = query.FilterByAuthor(bookQueryParameters.AuthorId)
                            .FilterByPublisher(bookQueryParameters.PublisherId)
                            .FilterByKeyword(bookQueryParameters.Keyword);

            var totalCount = await query.CountAsync();
            // var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginatedResult<BookSummaryDto> result = new()
            {
                Items = [.. books.Select(b => new BookSummaryDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ListPrice = b.ListPrice,
                    Discount = b.Discount,
                    ImagePath = b.ImagePath
                })],
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return result;
        }

        public async Task<BookDetailDto> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Book with ID {id} not found.");

            return new BookDetailDto
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Description = book.Description,
                ListPrice = book.ListPrice,
                Discount = book.Discount,
                Stock = book.Stock,
                PublicationDate = book.PublicationDate,
                ImagePath = book.ImagePath,
                AuthorId = book.Author.Id,
                AuthorName = book.Author.Name,
                PublisherId = book.Publisher.Id,
                PublisherName = book.Publisher.Name
            };
        }

        public async Task<int> CreateBookAsync(CreateBookRequest request)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == request.AuthorId)
                ?? throw new NotFoundException("Author does not exist.");
            var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == request.PublisherId)
                ?? throw new NotFoundException("Publisher does not exist.");
            if (await _context.Books.AnyAsync(b => b.Isbn == request.Isbn))
                throw new NotFoundException("ISBN already exists.");

            String imagePath = await _imageStorageService.UploadAsync(request.UploadedImage);

            var book = new Book
            {
                Isbn = request.Isbn,
                Title = request.Title,
                Description = request.Description,
                ListPrice = request.ListPrice,
                Discount = request.Discount,
                PublicationDate = request.PublicationDate,
                ImagePath = imagePath,
                Stock = request.Stock,
                Author = author,
                Publisher = publisher,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }
    }
}
