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
using AutoMapper;
using static System.Reflection.Metadata.BlobBuilder;
using Azure;

namespace BookStore.Services
{
    public class BooksService(ApplicationDbContext context, IImageStorageService imageStorageService, IUserInfoContext userInfoContext, IMapper mapper) : IBooksService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IImageStorageService _imageStorageService = imageStorageService;
        private readonly IUserInfoContext _userInfoContext = userInfoContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedResult<BookSummaryDto>> GetBooksAsync(BookQueryParameters bookQueryParameters)
        {
            var page = bookQueryParameters.Page;
            var pageSize = bookQueryParameters.PageSize;

            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .AsQueryable();

            // 權限與可見性處理
            if (_userInfoContext.IsAdmin)
            {
                if (bookQueryParameters.IncludeInvisibleBooks != true)
                {
                    // Admin 沒有特別要求看隱藏的書，也只看可見的
                    query = query.FilterByVisibility(true);
                }
            }
            else
            {
                // 一般用戶只能看可見的
                query = query.FilterByVisibility(true);
            }

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

        public async Task<int> CreateBookAsync(CreateBookRequest createRequest)
        {
            var author = await GetAuthorByIdAsync(createRequest.AuthorId);
            var publisher = await GetPublisherByIdAsync(createRequest.PublisherId);
            await EnsureIsbnUniqueAsync(createRequest.Isbn);

            String imagePath = await _imageStorageService.UploadAsync(createRequest.UploadedImage);

            var book = new Book
            {
                Isbn = createRequest.Isbn,
                Title = createRequest.Title,
                Description = createRequest.Description,
                ListPrice = createRequest.ListPrice,
                Discount = createRequest.Discount,
                PublicationDate = createRequest.PublicationDate,
                ImagePath = imagePath,
                Stock = createRequest.Stock,
                Author = author,
                Publisher = publisher,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, UpdateBookRequest updateRequest)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Book with ID {id} not found.");

            Author? author = null;
            Publisher? publisher = null;

            if (updateRequest.AuthorId is int authorId)
            {
                author = await GetAuthorByIdAsync(authorId);
            }

            if (updateRequest.PublisherId is int publisherId)
            {
                publisher = await GetPublisherByIdAsync(publisherId);
            }

            if (updateRequest.Isbn != null && !string.Equals(updateRequest.Isbn, book.Isbn))
            {
                await EnsureIsbnUniqueAsync(updateRequest.Isbn);
            }

            String imagePath = String.Empty;
            if (updateRequest.UploadedImage != null)
            {
                imagePath = await _imageStorageService.UploadAsync(updateRequest.UploadedImage);
            }

            _mapper.Map(updateRequest, book);
            book.Author = author ?? book.Author;
            book.Publisher = publisher ?? book.Publisher;
            if (!string.IsNullOrEmpty(imagePath))
            {
                book.ImagePath = imagePath;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
            if (author == null)
            {
                throw new NotFoundException("Author does not exist.");
            }
            return author;
        }

        private async Task<Publisher> GetPublisherByIdAsync(int publisherId)
        {
            var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == publisherId);
            if (publisher == null)
            {
                throw new NotFoundException("Publisher does not exist.");
            }
            return publisher;
        }

        private async Task EnsureIsbnUniqueAsync(string isbn)
        {
            if (await _context.Books.AnyAsync(b => b.Isbn == isbn))
            {
                throw new NotFoundException("ISBN already exists.");
            }
        }

        public async Task UpdateBooksVisibilityAsync(List<BookVisibilityUpdateRequest> requests) {

            var bookIds = requests.Select(r => r.BookId).ToList();

            var books = await _context.Books
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            foreach (var book in books)
            {
                var matchingRequest = requests.FirstOrDefault(r => r.BookId == book.Id);
                if (matchingRequest != null)
                {
                    book.IsVisible = matchingRequest.IsVisible;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CartBookItemDto>> GetBooksForCartAsync(List<int> ids)
        {
            var cartItems = await _context.Books
                                   .Where(book => ids.Contains(book.Id))
                                   .Select(book => new CartBookItemDto
                                   {
                                       Id = book.Id,
                                       Title = book.Title,
                                       UnitPrice = CountUnitPrice(book.ListPrice, book.Discount),
                                       Stock = book.Stock,
                                       ImagePath = book.ImagePath
                                   })
                                   .ToListAsync();
            return cartItems;
        }

        private static decimal CountUnitPrice(decimal listPrice, int discount)
        {
            return Math.Round(listPrice * (discount / 100m), 0);
        }
    }
}
