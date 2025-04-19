using BookStore.Core.Strategies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
