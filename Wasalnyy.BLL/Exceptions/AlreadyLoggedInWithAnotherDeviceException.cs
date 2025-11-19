using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Exceptions
{
    public class AlreadyLoggedInWithAnotherDeviceException : Exception
    {
        public AlreadyLoggedInWithAnotherDeviceException(string? message) : base(message)
        {
        }
    }
}
