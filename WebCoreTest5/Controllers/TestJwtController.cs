using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebCoreTest5.Converter;
using WebCoreTest5.Db;
using WebCoreTest5.Filters;
using WebCoreTest5.Model;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestJwtController : BaseController
    {
        private ApplicationDbContext _db = null;
        private IHttpContextAccessor _httpContextAccessor = null;

        public TestJwtController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            this._db = db;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Test1([FromForm] string name, [FromForm] string pwd)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(pwd))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, name)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // audience 用于让用户过期，进行校验
                string audience = name + pwd + DateTime.Now.ToString();

                // 需要把 用户 和 audience保存到redis里面，才能在 builder里面进行校验

                Console.WriteLine("login audience " + audience);
                var token = new JwtSecurityToken(
                    issuer: Const.Domain,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return BadRequest(new { message = "username or password is incorrect." });
            }
        }

        [JwtUser]
        [Authorize]
        [HttpGet]
        public ActionResult Test2()
        {
            var userInfo = _httpContextAccessor.HttpContext.User;

            return Content("hello jwt Name " + userInfo.Identity.Name);
        }
    }
    public class Const
    {
        /// <summary>
        /// 这里为了演示，写死一个密钥。实际生产环境可以从配置文件读取,这个是用网上工具随便生成的一个密钥
        /// </summary>
        public const string SecurityKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB";
        public const string Domain = "http://localhost:5000";
    }
}
