using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Model;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : BaseController
    {
        private IConfiguration config = null;

        public TestController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("hello,world!");
        }
    }
}
