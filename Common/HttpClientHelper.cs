namespace PointsMall.Common
{
    using Newtonsoft.Json;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// 公共使用类
    /// </summary>
    public static class HttpClientHelper
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 模拟get请求
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>String</returns>
        public static string HttpGet(Uri url)
        {
            string result = string.Empty;
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            using (var response = webRequest.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }




        /// <summary>
        /// 发送http get请求从另一个服务地址获取信息
        /// </summary>
        /// <param name="requestUri">uri</param>
        /// <returns>Response message</returns>
        public static async Task<T> GetAsync<T>(Uri requestUri)
        {
            if (requestUri != null)
            {

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var resp = await httpClient.GetAsync(requestUri);

                    if (resp == null || resp.IsSuccessStatusCode == false)
                    {
                        var errMsg = resp == null ? "返回为空" : await resp.Content.ReadAsStringAsync();
                        _logger.Log(LogLevel.Error, $"url:{requestUri}. Error:{errMsg}. Code:{resp.StatusCode}");
                        return default(T);
                    }

                    var jsonContent = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonContent);
                }

            }

            return default(T);
        }


 

        /// <summary>
        /// 发送post请求到另一个服务, 请求API不应该捕捉异常然后只记录日志,这样会导致业务部分成功
        /// </summary>
        /// <param name="uri">Uri路径</param>
        /// <param name="jsonData">post对象</param>
        /// <returns>Response message</returns>
        public static async Task<string> HttpPostJsonAsync(Uri uri, string jsonData)
        {
            if (uri == null)
                throw ExceptionHelper.InvalidArgumentException(nameof(uri));

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var resp = await httpClient.PostAsync(
                    uri,
                    new StringContent(jsonData,
                    Encoding.UTF8,
                    "application/json"));

                var responeContent = await resp.Content.ReadAsStringAsync();
                if (resp == null || resp.IsSuccessStatusCode == false)
                {
                    var errMsg = responeContent;
                    throw ExceptionHelper.UnknownException($"url:{uri}. Code:{(int)resp.StatusCode} {resp.StatusCode}. Error:{errMsg}. ");
                }
                return responeContent;
            }
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string HttpPostXml(Uri uri, string xml)
        {
            string result = string.Empty;

            WebRequest req = WebRequest.Create(uri);
            req.Method = "Post";
            req.ContentType = "text/xml; charset=utf-8";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(xml);
                sw.Flush();
            }
            using (var res = req.GetResponse())
            {
                using (var sr = new StreamReader(res.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// application/json数据格式模拟put请求
        /// </summary>
        /// <param name="uri">地址</param>
        /// <param name="data">数据</param>
        /// <returns>string</returns>
        public static string HttpPutJson(Uri uri, string data)
        {
            string result = string.Empty;

            WebRequest req = WebRequest.Create(uri);
            req.Method = "Put";
            req.ContentType = "application/json; charset=utf-8";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(data);
                sw.Flush();
            }

            using (var res = req.GetResponse())
            {
                using (var sr = new StreamReader(res.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }


            return result;
        }

        /// <summary>
        /// application/json数据格式模拟Delete请求
        /// </summary>
        /// <param name="uri">地址</param>
        /// <param name="data">数据</param>
        /// <returns>string</returns>
        public static string HttpDeleteJson(Uri uri, string data)
        {
            string result = string.Empty;

            WebRequest req = WebRequest.Create(uri);
            req.Method = "Delete";
            req.ContentType = "application/json; charset=utf-8";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(data);
                sw.Flush();
            }

            using (var res = req.GetResponse())
            {
                using (var sr = new StreamReader(res.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }


            return result;
        }

        /// <summary>
        /// 模拟put表单请求
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="putData">参数</param>
        /// <returns>String</returns>
        public static string HttpPut(Uri url, string putData)
        {
            string result = string.Empty;

            var data = Encoding.ASCII.GetBytes(putData);
            WebRequest res = WebRequest.Create(url);
            res.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            res.Method = "put";
            res.UseDefaultCredentials = true;
            res.ContentLength = data.Length;
            using (var stream = res.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var task = res.GetResponseAsync();
            WebResponse rep = task.Result;
            Stream respStream = rep.GetResponseStream();
            using (StreamReader reader = new StreamReader(respStream, Encoding.Default))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// 模拟delete表单请求
        /// </summary>
        /// <param name="url">路径</param>
        /// <param name="deleteData">参数</param>
        /// <returns>String</returns>
        public static string HttpDelete(Uri url, string deleteData)
        {
            string result = string.Empty;

            var data = Encoding.ASCII.GetBytes(deleteData);
            WebRequest res = WebRequest.Create(url);
            res.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            res.Method = "delete";
            res.UseDefaultCredentials = true;
            res.ContentLength = data.Length;
            using (var stream = res.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var task = res.GetResponse();
            WebResponse rep = task;
            Stream respStream = rep.GetResponseStream();
            using (StreamReader reader = new StreamReader(respStream, Encoding.Default))
            {
                result = reader.ReadToEnd();
            }


            return result;
        }
    }

    internal enum EnumHttpContentType
    {
        /// <summary>
        /// 表单数据(Form)
        /// </summary>
        FormData,
        /// <summary>
        /// Json数据(Body)
        /// </summary>
        JsonData,
    }
}