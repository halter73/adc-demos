using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasicWeb.Controllers
{
    [Route("api/[controller]")]
    public class PrimeController : Controller
    {
        private static readonly object _primeLock = new object();

        // GET api/prime/5
        [HttpGet("{number}")]
        public bool Get(long number)
        {
            return IsPrime(number);
        }

        public static bool IsPrime(long number)
        {
            lock (_primeLock)
	    {
	        //Console.WriteLine("Finding prime of '{0}'.", number);
                if (number == 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0)  return false;
            
                var boundary = (long)Math.Floor(Math.Sqrt(number));
            
                for (long i = 3; i <= boundary; i+=2)
                {
                    if (number % i == 0)  return false;
                }
            
                return true;        
	    }
        }
    }
}
