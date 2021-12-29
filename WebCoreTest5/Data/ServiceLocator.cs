using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Data
{
    /// <summary>
    /// 本地服务对象，single
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// single对象
        /// </summary>
        public static IServiceProvider Instance { get; set; }
    }
}
