using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5
{
    /// <summary>
    /// 后台任务帮助类
    /// </summary>
    public static class BackgroundServicesHelper
    {
        /// <summary>
        /// 反射取得所有的业务逻辑类
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        private static Type[] GetAllChildClass(Type baseType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
            //取得实现了某个接口的类
            //.SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISecurity))))  .ToArray();
            //取得继承了某个类的所有子类
            .SelectMany(a => a.GetTypes().Where(t => t.BaseType == baseType))
            .ToArray();

            return types;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Type[] GetAllBackgroundService()
        {
            return GetAllChildClass(typeof(BackgroundService));
        }

        /// <summary>
        /// 自动增加后台任务.所有继承自BackgroundService的类都会自动运行
        /// IIS有自动回收机制，no request no target
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            //services.AddHostedService(); //asp.net core 应该是这个.
            //或者  单为方便循环自动创建, 所以改成使用AddTransient 也一样可以使用.
            //services.AddTransient();  
            //services.AddTransient(typeof(Microsoft.Extensions.Hosting.IHostedService),backtype);
            //var backtypes = BackgroundServicesHelper.GetAllBackgroundService();
            //foreach (var backtype in backtypes)
            //{
            //    services.AddTransient(typeof(Microsoft.Extensions.Hosting.IHostedService),backtype);
            //}
            var backtypes = GetAllBackgroundService();
            foreach (var backtype in backtypes)
            {
                // AddSingleton  AddTransient
                services.AddSingleton(typeof(Microsoft.Extensions.Hosting.IHostedService), backtype);
            }

            return services;
        }
    }
}
