using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public interface IUserInfoContext
    {
        string UserId { get; }
        RoleType UserRole { get; }
        bool IsAdmin { get; }
    }
}
