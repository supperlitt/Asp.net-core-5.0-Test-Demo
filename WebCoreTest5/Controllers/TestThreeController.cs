using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebCoreTest5.Converter;
using WebCoreTest5.Db;
using WebCoreTest5.Model;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestThreeController : BaseController
    {
        protected ApplicationDbContext _db = null;

        public TestThreeController(ApplicationDbContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var info = this._db.datas.FirstOrDefault();

            return Content("info " + (info == null ? "null" : info.data_key + " " + info.data_value));
        }

        [HttpGet]
        public IActionResult Test1()
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = 1,
                name = "aaaaaa",
                age = 30,
                login_time = DateTime.Now
            });

            return Content(json);
        }

        [HttpGet]
        public IActionResult Test2()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
            };
            options.Converters.Add(new DatetimeJsonConverter());

            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = 1,
                name = "aaaaaa",
                age = 30,
                login_time = DateTime.Now
            }, options);

            return Content(json);
        }
    }
}
