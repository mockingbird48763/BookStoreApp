using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.DTO.Request;

namespace BookStore.Services
{
    public interface IMemberService
    {
        Task RegisterAsync(RegisterMemberRequest request);
    }
}
