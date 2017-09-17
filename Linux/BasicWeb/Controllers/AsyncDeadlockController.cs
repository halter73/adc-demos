using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasicWeb.Controllers
{
    [Route("api/[controller]")]
    public class AsyncDeadlockController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return DbQuery().Result;
        }

        // [HttpGet]
        // public async Task<string> GetAsync()
        // {
        //     return await DbQuery();
        // }

        private async Task<string> DbQuery()
        {
            await Task.Delay(1000);
            return "Hello World!";
        }
    }
}
