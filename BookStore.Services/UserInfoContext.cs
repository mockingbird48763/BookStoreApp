using BookStore.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class UserInfoContext(IHttpContextAccessor httpContextAccessor) : IUserInfoContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string UserId
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            }
        }

        public RoleType UserRole
        {
            get
            {
                var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

                return roleClaim switch
                {
                    "Admin" => RoleType.Admin,
                    "User" => RoleType.User,
                    _ => RoleType.Guest, // 默認為 Guest
                };
            }
        }

        public bool IsAdmin
        {
            get
            {
                return UserRole == RoleType.Admin;
            }
        }
    }
}
