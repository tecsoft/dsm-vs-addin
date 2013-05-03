using System;
using System.Collections.Generic;
using System.Text;

namespace Tcdev.Dsm
{
    public class DsmException : ApplicationException
    {
        public DsmException()
            : base()
        {
        }
        public DsmException(string message)
            : base(message)
        {
        }

        public DsmException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
