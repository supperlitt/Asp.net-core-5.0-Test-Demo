using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebCoreTest5.Filters
{
    public class JwtUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var test = context.HttpContext.Request.Path;
            string bearer = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(bearer) || !bearer.Contains("Bearer")) return;
            string[] jwt = bearer.Split(' ');
            var tokenObj = new JwtSecurityToken(jwt[1]);

            var claimsIdentity = new ClaimsIdentity(tokenObj.Claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            context.HttpContext.User = claimsPrincipal;
        }
    }
}
