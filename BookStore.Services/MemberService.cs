using BookStore.Data;
using BookStore.DTO.Request;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext _context;
        private Role? _defaultRole;


        public MemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAsync(RegisterMemberRequest request)
        {
            // 檢查電子郵件是否已經存在
            if (await _context.Members.AnyAsync(m => m.Email == request.Email))
            {
                throw new Exception("Email 已存在");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var role = await GetDefaultRoleAsync();

            var member = new Member
            {
                Email = request.Email,
                Password = hashedPassword,
                // Roles = new List<Role> { role }
                Roles = [role]
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }

        private async Task<Role> GetDefaultRoleAsync()
        {
            if (_defaultRole == null)
            {
                _defaultRole = await _context.Roles.FirstAsync(r => r.Id == 2);
            }
            return _defaultRole;
        }
    }
}
