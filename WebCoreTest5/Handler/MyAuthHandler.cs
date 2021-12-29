using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebCoreTest5.Db;

namespace WebCoreTest5
{
    /// <summary>
    /// 身份校验
    /// </summary>
    public class MyAuthHandler : IAuthenticationHandler
    {
        /// <summary>
        /// 就一个字典的Key没啥用处
        /// </summary>
        public static readonly string SchemeName = "token";

        /// <summary>
        /// schema
        /// </summary>
        AuthenticationScheme _scheme;

        /// <summary>
        /// 上下文
        /// </summary>
        HttpContext _context;

        /// <summary>
        /// 初始化认证
        /// </summary>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 认证处理
        /// </summary>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            var token = _context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.Fail("未登陆"));
            }

            var tokenStr = token.ToString();
            var DB = (ApplicationDbContext)_context.RequestServices.GetService(typeof(ApplicationDbContext));

            // 从 数据库 读取 用户，根据 token,
            // 此时就可以得到 token了，同时如果用户接入，就可以得到用户身份了。通过 ActionFilter context.HttpContext.User
            var ticket = GetAuthTicket("user_name", "user_phone");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        /// <summary>
        /// 创建一个授权的票据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private AuthenticationTicket GetAuthTicket(string name, string role)
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
            }, "My_Auth");

            var principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, _scheme.Name);
        }

        /// <summary>
        /// 权限不足时的处理
        /// </summary>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return Task.CompletedTask;
        }

        /// <summary>
        /// 未登录时的处理
        /// </summary>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return Task.CompletedTask;
        }
    }
}
