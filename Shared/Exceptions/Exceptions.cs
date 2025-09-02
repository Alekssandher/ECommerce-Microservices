using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public static class Exceptions
    {
        public class NotFoundException(string message) : Exception(message)
        {
        }

        public class BadRequestException(string message) : Exception(message)
        {
            
        }
    }
}