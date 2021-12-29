﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Db;

namespace WebCoreTest5
{
    public class MyFactory : IFactory
    {
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public ApplicationDbContext DB { get; }

        /// <summary>
        /// Http上下文
        /// </summary>
        public IHttpContextAccessor httpContextAccessor { get; }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration config { get; }

        public MyFactory(IHttpContextAccessor httpContextAccessor, IConfiguration config, ApplicationDbContext DB)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.config = config;
            this.DB = DB;
        }
    }
}
