using AutoMapper;
using BookStore.DTO.Request;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStore.DTO.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            // 定義 CreateBookRequest 和 Book 之間的映射
            // src -> dest
            CreateMap<UpdateBookRequest, Book>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // 可配置其他映射
            // CreateMap<BookDto, Book>()
        }
    }
}
