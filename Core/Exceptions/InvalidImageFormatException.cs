using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Exceptions
{
    public class InvalidImageFormatException(string message) : Exception(message)
    {
    }
}
