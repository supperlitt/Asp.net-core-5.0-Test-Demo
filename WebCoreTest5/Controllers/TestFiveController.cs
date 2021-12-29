using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestFiveController : BaseController2
    {
        public TestFiveController(IFactory factory) : base(factory)
        {

        }

        [HttpGet]
        public string Test()
        {
            return factory.config.GetSection("ConnectionStrings:TestDb").Value;
        }
    }
}
