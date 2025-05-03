using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Member?> GetByEmailAsync(string email)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task AddAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }
    }
}
