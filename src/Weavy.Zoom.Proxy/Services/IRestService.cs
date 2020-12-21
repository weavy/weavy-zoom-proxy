using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Weavy.Zoom.Proxy.Services
{
    public interface IRestService
    {
        Task<HttpResponseMessage> Post(string uri, string body);
    }
}
