using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCoreTest5.Http
{
    public class MyHttpClient
    {
        private HttpClient _httpClient { get; set; }

        public MyHttpClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
    }
}
