using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Controllers
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// 异常入口
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context.Error != null)
            {
                // Logs.WriteLog(LogType.Error, "ex " + context.Error.ToString());
            }
            else
            {
                // Logs.WriteLog(LogType.Error, "ex null");
            }

            return Problem();
        }
    }
}
