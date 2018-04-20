using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer.HttpProxy
{
    public class My404Hander : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode,
                                      NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = new TextResponse("not a supported uri");
            response.StatusCode = statusCode;
            context.Response = response;
        }
    }
}
