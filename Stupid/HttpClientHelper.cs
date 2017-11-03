using Stupid.SomeConvert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Stupid
{
    /// <summary>
    /// 获取Url中的Json信息，一般用于接口
    /// </summary>
    public class HttpClientHelper
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }

            return null;
        }

        /// <summary>
        /// Get请求，接收数据后转换成T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetResponse<T>(string url)
            where T : class, new()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.JsonDeserialize<T>(s);
            }
            return result;
        }

        /// <summary>
        /// 获取GET信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms"></param>
        /// <param name="htmlEncoder"></param>
        /// <returns></returns>
        public static string HttpGet(string url, string parms, string htmlEncoder = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (parms == "" ? "" : "?") + parms);
            request.Method = "GET";
            request.ContentType = "text/html";
            //添加cookie防止重定向时候出现异常
            // request.AllowAutoRedirect = true;
            //System.Net.CookieContainer cookieContext = new CookieContainer();
            //request.CookieContainer = cookieContext;
            //request.KeepAlive = true;
            //request.Host = "www.1399p.com";
            //加入浏览器标识防止获取数据为空。

            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            {
                return true;
            };

            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            #region MyRegion
            //System.Net.WebRequest wReq = System.Net.WebRequest.Create(url);

            //System.Net.WebResponse wResp = wReq.GetResponse();
            //System.IO.Stream respStream = wResp.GetResponseStream();
            //// Dim reader As StreamReader = New StreamReader(respStream)
            //using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(htmlEncoder == "" ? "ASCII" : htmlEncoder)))
            //{
            //    return reader.ReadToEnd();
            //} 
            #endregion
            var header = request.GetResponse().Headers;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var heards = request.GetResponse().Headers;
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(htmlEncoder == "" ? "ASCII" : htmlEncoder));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }


        public static void g()
        {
         
        }
        /// <summary>
        /// 获取POST信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parms">类似username=lxh&amp;password=123456&amp;vcode=5-100&amp;vctime=0.06349646166503276&amp;kd=16&amp;cd=2</param>
        /// <returns></returns>
        public static string HttpPost(string url, string parms)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = parms.Length;

            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            {
                return true;
            };

            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(parms);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }
    }
}
