using BookStore.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(String email, String password);
    }
}
