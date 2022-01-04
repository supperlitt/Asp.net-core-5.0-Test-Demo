using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string str = null;
            int? len = str?.Length;
            CreateHostBuilder(args).Build().Run();
        }

        private static Dictionary<string, string> arrayDict = new Dictionary<string, string>
        {
            {"array:entries:0", "value0"},
            {"array:entries:1", "value1"},
            {"array:entries:2", "value2"},
            //              3   Skipped
            {"array:entries:4", "value4"},
            {"array:entries:5", "value5"}
        };

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // 配置 额外的配置文件
                .ConfigureAppConfiguration((context, config) =>
                {
                    //    config.AddJsonFile("MySubsection.json",
                    //     optional: true,
                    //     reloadOnChange: true);
                    config.AddInMemoryCollection(arrayDict);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
