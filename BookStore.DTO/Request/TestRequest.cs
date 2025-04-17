using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class TestRequest
    {
        /// <summary>
        /// The name
        /// </summary>
        /// <example>Jack</example>
        public required string Name { get; set; }
    }
}
