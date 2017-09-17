using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasicWeb.Controllers
{
    [Route("api/[controller]")]
    public class DeadlockController : Controller
    {
        private static readonly ServiceA _serviceA = new ServiceA();
        private static readonly ServiceB _serviceB = new ServiceB();

        private static int _counter = 0;

        // GET api/deadlock
        [HttpGet]
        public int Get()
        {
            if (Interlocked.Increment(ref _counter) % 2 == 0)
            {
                return _serviceA.GetSum(_serviceB);
            }
            else
            {
                return _serviceB.GetSum(_serviceA);
            }
        }

        private class ServiceA
        {
            private const int _a = 7;
            private readonly object _lockA = new object();


            public int GetSum(ServiceB serviceB)
            {
                lock (_lockA)
                {
                    return _a + serviceB.GetValue();
                }
            }

            public int GetValue()
            {
                lock (_lockA)
                {
                    return _a;
                }
            }
        }

        private class ServiceB
        {
            private const int _b = 3;
            private readonly object _lockB = new object();

            public int GetSum(ServiceA serviceA)
            {
                lock (_lockB)
                {
                    return _b + serviceA.GetValue();
                }
            }

            public int GetValue()
            {
                lock (_lockB)
                {
                    return _b;
                }
            }
        }
    }
}
