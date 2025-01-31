using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Exceptions
{
    public class GlobalException: Exception
    {
        public GlobalException(string message) :base(message)
        {
        }
    }
}
