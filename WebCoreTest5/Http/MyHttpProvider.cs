using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCoreTest5.Http
{
    public class MyHttpProvider
    {
        private IHttpClientFactory _httpClientFactory { get; set; }
        private Dictionary<string, MyHttpClient> _Clients = new Dictionary<string, MyHttpClient>();

        public MyHttpProvider(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        public MyHttpClient GetClient(string id)
        {
            lock (_Clients)
            {
                _Clients.TryGetValue(id, out MyHttpClient client);
                if (client == null)
                {
                    client = new MyHttpClient(_httpClientFactory.CreateClient(nameof(MyHttpProvider)));
                    _Clients.TryAdd(id, client);
                }

                return client;
            }
        }
    }
}
