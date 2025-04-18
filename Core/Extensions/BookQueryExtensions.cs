using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Extensions
{
    public static class BookQueryExtensions
    {
        public static IQueryable<Book> FilterByAuthor(this IQueryable<Book> query, int? authorId)
        {
            if (authorId.HasValue)
            {
                query = query.Where(b => b.Author.Id == authorId.Value);
            }
            return query;
        }

        public static IQueryable<Book> FilterByPublisher(this IQueryable<Book> query, int? publisherId)
        {
            if (publisherId.HasValue)
            {
                query = query.Where(b => b.Publisher.Id == publisherId.Value);
            }
            return query;
        }

        public static IQueryable<Book> FilterByKeyword(this IQueryable<Book> query, string? keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(b => b.Title.Contains(keyword) || b.Description.Contains(keyword));
            }
            return query;
        }
    }
}
