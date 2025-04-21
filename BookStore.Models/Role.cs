using BookStore.Models;
using BookStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Role
{
    public int Id { get; set; }

    public required RoleType Name { get; set; }

    public ICollection<Member> Members { get; set; } = new List<Member>();
}
