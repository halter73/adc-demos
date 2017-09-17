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
        private static readonly ConcurrentDictionary<string, int> _dict = new ConcurrentDictionary<string, int>();

        [HttpGet]
        public int Get()
        {
            HttpContext.Response.Headers["Connection"] = "close";
            return _dict.AddOrUpdate(HttpContext.Connection.Id, 0, (id, count) => count + 1);
        }

        // private static readonly ConcurrentDictionary<string, (HttpContext, int)> _dict = new ConcurrentDictionary<string, (HttpContext, int)>();

        // [HttpGet]
        // public int Get()
        // {
        //     HttpContext.Response.Headers["Connection"] = "close";
        //     return _dict.AddOrUpdate(HttpContext.Connection.Id, (HttpContext, 0), (id, tuple) => (tuple.Item1, tuple.Item2 + 1)).Item2;
        // }
    }
}
