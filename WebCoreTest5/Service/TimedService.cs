using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCoreTest5.Db;
using WebCoreTest5.Model;

namespace WebCoreTest5
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimedService : BackgroundService
    {
        /// <summary>
        /// db 对象
        /// </summary>
        protected readonly ApplicationDbContext context = null;

        /// <summary>
        /// 定时服务
        /// </summary>
        /// <param name="factory"></param>
        public TimedService(IServiceScopeFactory factory)
        {
            this.context = factory.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Worker running at: {DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                try
                {
                    DateTime checkDate = DateTime.Now.Date;
                    // 读取并执行任务
                    // var dayList = context.Set<data_info>()
                }
                catch (Exception e)
                {
                    // Logs.WriteLog(LogType.Error, "check task error " + e.ToString());
                }

                // 1分钟判断一次
                await Task.Delay(5 * 60000, stoppingToken);
            }
        }
    }
}
