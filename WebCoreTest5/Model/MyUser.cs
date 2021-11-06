using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Model
{
    public class MyUser : IdentityUser
    {
        public string Pass { get; set; }

        public string DisplayName { get; set; }

        public DateTime RegisteredTime { get; set; }
    }
}
