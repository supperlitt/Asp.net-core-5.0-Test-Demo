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
    public class BaseController2 : ControllerBase
    {
        protected IFactory factory { get; set; }

        public BaseController2(IFactory factory)
        {
            this.factory = factory;
        }
    }
}
