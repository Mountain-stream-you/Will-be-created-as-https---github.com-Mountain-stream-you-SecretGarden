using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace PointsMall.Common
{
    public static class HttpContextHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static HttpMethod GetHttpMethod(string input)
        {
            HttpMethod method = null;
            switch (input.ToLower())
            {
                case "get":
                    method = HttpMethod.Get;
                    break;
                case "post":
                    method = HttpMethod.Post;
                    break;
                case "put":
                    method = HttpMethod.Put;
                    break;
                case "delete":
                    method = HttpMethod.Delete;
                    break;
            }
            return method;
        }
    }
}
