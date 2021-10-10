using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NotEnoughCommercialsToAllocateException : Exception
    {
        public NotEnoughCommercialsToAllocateException(string message)
       : base(message)
        {
        }
    }

    public class MaximumLimitIsNotDefinedForAllBreaks :Exception
    {
        public MaximumLimitIsNotDefinedForAllBreaks(string message)
              : base(message)
        {
        }
    }

    public class AllocationProcessingException : Exception
    {
        public AllocationProcessingException(string message)
       : base(message)
        {
        }
    }
}
