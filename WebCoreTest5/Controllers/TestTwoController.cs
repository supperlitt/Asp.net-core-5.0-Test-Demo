using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Model;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestTwoController : BaseController
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;
        ILogger _logger;

        public TestTwoController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Content("Test2");
        }

        [HttpGet]
        public IActionResult Test1()
        {
            return Content("GetUserId " + _userManager.GetUserId(this.User));
        }

        [HttpGet]
        public IActionResult Test2()
        {
            var userInfo = _userManager.GetUserAsync(this.User);
            var json = System.Text.Json.JsonSerializer.Serialize(userInfo.Result);
            return Content("GetUserAsync " + json);
        }
    }
}
