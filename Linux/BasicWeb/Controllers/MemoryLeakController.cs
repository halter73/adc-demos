using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasicWeb.Controllers
{
    [Route("api/[controller]")]
    public class MemoryLeakController : Controller
    {
        private static readonly Holder _holder = new Holder();

        [HttpGet]
        public int Get()
        {
            return _holder.Dictionary.AddOrUpdate(HttpContext.TraceIdentifier, 0, (id, count) => count + 1);
        }

        private class Holder
        {
            public readonly ConcurrentDictionary<string, int> Dictionary = new ConcurrentDictionary<string, int>();
        }
    }
}
