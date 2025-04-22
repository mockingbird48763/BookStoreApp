using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Models;

namespace BookStore.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByEmailAsync(string email);
        Task AddAsync(Member member);
    }
}
