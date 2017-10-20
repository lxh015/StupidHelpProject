using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHandle
{
    public class LogException : Exception
    {
        public LogException() : base()
        {
        }

        public LogException(string message) : base(message)
        {

        }

        public LogException(Exception innerException, string message = "") : base(message, innerException)
        {

        }
    }
}
