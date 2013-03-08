using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.AppCode.Networking.SectorService
{
    public class MyServiceException : ApplicationException
    {
        public MyServiceException()
        {
        }

        public MyServiceException(string message)
            : base(message)
        {
        }

        public MyServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
