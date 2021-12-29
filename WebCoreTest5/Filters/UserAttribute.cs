using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebCoreTest5
{
    /// <summary>
    /// 普通用户权限校验
    /// </summary>
    public class UserAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// action 的处理
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // context.HttpContext.User 可以拿到用户信息，然后判断用户权限
            context.Result = new ContentResult() { Content = "无权限" };
        }
    }
}
