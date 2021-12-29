using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Data;
using WebCoreTest5.Db;

namespace WebCoreTest5.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public ApplicationDbContext DB { get; set; }

        /// <summary>
        /// Http上下文
        /// </summary>
        protected IHttpContextAccessor httpContextAccessor { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        protected IConfiguration config { get; set; }

        public BaseController()
        {
            config = (IConfiguration)ServiceLocator.Instance.GetService(typeof(IConfiguration));
            httpContextAccessor = (IHttpContextAccessor)ServiceLocator.Instance.GetService(typeof(IHttpContextAccessor));
            if (httpContextAccessor != null)
            {
                DB = (ApplicationDbContext)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext));
                string user_phone = httpContextAccessor.HttpContext.User.Identity.Name;
                if (user_phone != null)
                {
                    // 此处可以从DB读取到用户信息，然后初始化用户到BaseController中了。。。牛b
                }
            }
        }
    }
}
