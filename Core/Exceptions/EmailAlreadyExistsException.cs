using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Exceptions
{
    public class EmailAlreadyExistsException(string message) : Exception(message)
    {
    }
}
