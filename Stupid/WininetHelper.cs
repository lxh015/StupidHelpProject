using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Stupid
{
    public class WininetHelper
    {
        #region 调用例子
        //开始准备测试 cookie获取功能

        string warp = "\r\n";
        //public string GetWininet(string pUserAgent, string url, string pReferer, Encoding encoding, string pCookieCollection)
        //{
        //    //pCookie = "";
        //    {
        //        WininetHelper wininet = new WininetHelper();
        //        wininet.UserAgent = pUserAgent;
        //        //wininet.WininetTimeOut = 60000;

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("Accept-Language: zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3").Append(warp);
        //        sb.Append("Accept: */*").Append(warp);
        //        if (!string.IsNullOrEmpty(pCookieCollection))
        //            sb.Append("Cookie: ").Append(pCookieCollection).Append(warp);
        //        if (!string.IsNullOrEmpty(pReferer))
        //            sb.Append("Referer: " + pReferer).Append(warp);
        //        //sb.Append("Connection: keep-alive").Append(warp);

        //        return wininet.GetDataPro(wininet.GetHtmlPro(url, string.Empty, null, pCookieCollection, sb.ToString(), false), encoding);
        //    }
        //}

        //public string PostWininet(string pUserAgent, string url, string postdata, string pReferer, Encoding encoding, string pCookieCollection)
        //{
        //    {
        //        WininetHelper wininet = new WininetHelper();
        //        wininet.UserAgent = pUserAgent;
        //        //wininet.WininetTimeOut = 60000;

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("Content-Type: application/x-www-form-urlencoded; charset=UTF-8").Append(warp);
        //        sb.Append("Accept: application/json, text/javascript, */*; q=0.01").Append(warp);
        //        if (!string.IsNullOrEmpty(pCookieCollection))
        //            sb.Append("Cookie: ").Append(pCookieCollection).Append(warp);
        //        if (!string.IsNullOrEmpty(pReferer))
        //            sb.Append("Referer: " + pReferer).Append(warp);

        //        return wininet.GetDataPro(wininet.GetHtmlPro(url, postdata, null, pCookieCollection, sb.ToString(), false), encoding);
        //    }
        //}

        #endregion


        private int _WininetTimeOut = 0;
        public string UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";

        /// <summary>
        /// 设置访问超时
        /// </summary>
        public int WininetTimeOut { get { return this._WininetTimeOut; } set { this._WininetTimeOut = value; } }


        #region Cookie操作例子
        private Dictionary<string, string> CookieDic { set; get; }

        private List<KeyValuePair<K, V>> CopyKeyValue<K, V>(Dictionary<K, V> pDic)
        {
            return new List<KeyValuePair<K, V>>(pDic);
        }

        private string getCookieStr()
        {
            StringBuilder ret = new StringBuilder();
            List<KeyValuePair<string, string>> strlist = CopyKeyValue(CookieDic);
            int cnt = strlist.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (cnt - 1 == i)
                    ret.Append(string.Format("{0}={1};", strlist[i].Key, strlist[i].Value));
                else
                    ret.Append(string.Format("{0}={1}; ", strlist[i].Key, strlist[i].Value));
            }

            return ret.ToString();

        }

        //cookie 字符串 Domain=.*?Expires
        //取出 result.Cookie = this.response.Headers["set-cookie"]
        private void setCookie(string s = "")
        {
            if (string.IsNullOrEmpty(s)) { return; }
            Regex reg = new Regex(",(?<Key>.*?)=(?<Value>.*?);.*?Domain=.*?Expires=.*?,", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            MatchCollection marticles = reg.Matches(string.Format(",{0},", s));

            foreach (Match m in marticles)
            {
                string Key = m.Groups["Key"].Value.ToString();
                string Value = m.Groups["Value"].Value.ToString();

                if (CookieDic.ContainsKey(Key))
                    CookieDic[Key] = Value;
                else
                    CookieDic.Add(Key, Value);
            }
        }

        #endregion


        public string CookieToString(CookieContainer cc)
        {
            List<Cookie> list = new List<Cookie>();
            Hashtable hashtable = (Hashtable)cc.GetType().InvokeMember("m_domainTable", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, cc, new object[0]);
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in hashtable.Values)
            {
                SortedList list2 = (SortedList)obj2.GetType().InvokeMember("m_list", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, obj2, new object[0]);
                foreach (CookieCollection cookies in list2.Values)
                {
                    foreach (Cookie cookie in cookies)
                    {
                        builder.Append(cookie.Name).Append("=").Append(cookie.Value).Append(";");
                    }
                }
            }
            return builder.ToString();
        }

        public List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> list = new List<Cookie>();
            Hashtable hashtable = (Hashtable)cc.GetType().InvokeMember("m_domainTable", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, cc, new object[0]);
            foreach (object obj2 in hashtable.Values)
            {
                SortedList list2 = (SortedList)obj2.GetType().InvokeMember("m_list", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, obj2, new object[0]);
                foreach (CookieCollection cookies in list2.Values)
                {
                    foreach (Cookie cookie in cookies)
                    {
                        list.Add(cookie);
                    }
                }
            }
            return list;
        }

        public string GetCookies(string url)
        {
            uint pcchCookieData = 0x100;
            StringBuilder pchCookieData = new StringBuilder((int)pcchCookieData);
            if (!InternetGetCookieEx(url, null, pchCookieData, ref pcchCookieData, 0x2000, IntPtr.Zero))
            {
                if (pcchCookieData < 0)
                {
                    return null;
                }
                pchCookieData = new StringBuilder((int)pcchCookieData);
                if (!InternetGetCookieEx(url, null, pchCookieData, ref pcchCookieData, 0x2000, IntPtr.Zero))
                {
                    return null;
                }
            }
            return (pchCookieData.ToString() + ";");
        }

        public void SetIeCookie(string GetUrl, string NewCookie)
        {
            StringBuilder data = new StringBuilder(new string(' ', 2048));
            int length = data.Length;
            bool flag = InternetGetCookie(GetUrl, null, data, ref length);
            foreach (string str in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies)))
            {
                if (str.ToLower().IndexOf(GetUrl) > 0)
                {
                    System.IO.File.Delete(GetUrl);
                }
            }
            foreach (string str2 in NewCookie.Split(new char[] { ';' }))
            {
                string[] strArray = str2.Split(new char[] { '=' });
                string lbszCookieName = strArray[0];
                string lpszCookieData = strArray[1] + ";expires=Sun,22-Feb-2099 00:00:00 GMT";
                InternetSetCookie(GetUrl, lbszCookieName, lpszCookieData);
                InternetSetCookie(GetUrl, lbszCookieName, lpszCookieData);
                InternetSetCookie(GetUrl, lbszCookieName, lpszCookieData);
            }
        }

        public CookieContainer StringToCookie(string url, string cookie)
        {
            string[] strArray = cookie.Split(new char[] { ';' });
            CookieContainer container = new CookieContainer();
            foreach (string str in strArray)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Cookie cookie2 = new Cookie
                    {
                        Name = str.Split(new char[] { '=' })[0].Trim(),
                        Value = str.Split(new char[] { '=' })[1].Trim(),
                        Domain = url
                    };
                    container.Add(cookie2);
                }
            }
            return container;
        }


        #region 旧的获取方法

        private MemoryStream GetHtml(string Url, byte[] Postdata, StringBuilder Header = null)
        {
            try
            {
                Thread thread;
                ThreadStart start = null;
                ThreadStart start2 = null;
                Uri uri = new Uri(Url);
                string getMethod = "GET";
                if (Postdata.Length != 0)
                {
                    getMethod = "POST";
                }
                string strAppName = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";//"WinInetGet/0.1";
                int ulSession = InternetOpen(strAppName, 1, "", "", 0);//初始化一个应用程序
                if (ulSession == 0)
                {
                    InternetCloseHandle(ulSession);
                    return null;
                }
                int num2 = InternetConnect(ulSession, uri.Host, uri.Port, "", "", 3, 0, 0);//建立 Internet 的连接
                if (num2 == 0)
                {
                    InternetCloseHandle(num2);
                    InternetCloseHandle(ulSession);
                    return null;
                }
                int dwflags = -2147483632;//0x80000010
                if (Url.Substring(0, 5) == "https")
                {
                    dwflags = -2139095024;//|= 0x800000;
                }
                else
                {
                    dwflags = -2147467248;//|= 0x4000;
                }
                int hRequest = HttpOpenRequest(num2, getMethod, uri.PathAndQuery, "HTTP/1.1", "", "", dwflags, 0);
                if (hRequest == 0)
                {
                    InternetCloseHandle(hRequest);
                    InternetCloseHandle(num2);
                    InternetCloseHandle(ulSession);
                    return null;
                }
                StringBuilder sb = new StringBuilder();
                if (Header == null)
                {
                    sb.Append("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n");
                    sb.Append("Content-Type:application/x-www-form-urlencoded\r\n");
                    sb.Append("Accept-Language:zh-cn\r\n");
                    sb.Append("Referer:" + Url);
                }
                else
                {
                    sb = Header;
                }
                if (string.Equals(getMethod, "GET", StringComparison.OrdinalIgnoreCase))//GET
                {
                    if (this._WininetTimeOut > 0)
                    {
                        if (start == null)
                        {
                            start = delegate
                            {
                                HttpSendRequestA(hRequest, sb.ToString(), sb.Length, null, 0);
                            };
                        }
                        thread = new Thread(start);
                        thread.IsBackground = true;
                        thread.Start();
                        thread.Join(this._WininetTimeOut);
                    }
                    else
                    {
                        HttpSendRequestA(hRequest, sb.ToString(), sb.Length, null, 0);
                    }
                }
                else if (this._WininetTimeOut > 0)//post
                {
                    if (start2 == null)
                    {
                        start2 = delegate
                        {
                            HttpSendRequestA(hRequest, sb.ToString(), sb.Length, Postdata, Postdata.Length);
                        };
                    }
                    thread = new Thread(start2);
                    thread.IsBackground = true;
                    thread.Start();
                    thread.Join(this._WininetTimeOut);
                }
                else
                {
                    HttpSendRequestA(hRequest, sb.ToString(), sb.Length, Postdata, Postdata.Length);//post
                }
                int revSize = 0;
                byte[] pByte = new byte[0x400];
                MemoryStream stream = new MemoryStream();

                goto NextStep;
                Label_030E:
                if (InternetReadFile(hRequest, pByte, 0x400, out revSize) && (revSize > 0))//打开的句柄中读取数据
                {
                    stream.Write(pByte, 0, revSize);
                }
                else
                {
                    goto goout;
                }
                NextStep:
                goto Label_030E;

                goout:

                InternetCloseHandle(hRequest);
                InternetCloseHandle(num2);
                InternetCloseHandle(ulSession);
                return stream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 给定url获取网页数据
        /// </summary>
        /// <param name="Url"></param>
        /// <returns>null or 字符串</returns>
        public string GetData(string Url)
        {
            using (MemoryStream stream = this.GetHtml(Url, null, null))
            {
                if (stream != null)
                {
                    Match match = Regex.Match(Encoding.Default.GetString(stream.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    string str = (match.Groups.Count > 1) ? match.Groups[2].Value.ToUpper().Trim() : string.Empty;
                    if (str.IndexOf("\"") > 0)
                    {
                        str = str.Split(new char[] { '"' })[0];
                    }
                    if ((str.Length > 2) && (str.IndexOf("UTF") != -1 || str.IndexOf("UTF-8") != -1))
                    {
                        return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
                    }
                    return Encoding.GetEncoding("GBK").GetString(stream.ToArray());
                }
                return null;
            }
        }

        public string PostData(string Url, string postData, StringBuilder Header = null)
        {
            using (MemoryStream stream = this.GetHtml(Url, Encoding.UTF8.GetBytes(postData), Header))
            {
                Match match = Regex.Match(Encoding.Default.GetString(stream.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string str = (match.Groups.Count > 1) ? match.Groups[2].Value.ToUpper().Trim() : string.Empty;
                if (str.IndexOf("\"") > 0)
                {
                    str = str.Split(new char[] { '"' })[0];
                }
                if ((str.Length > 2) && (str.IndexOf("UTF-8") != -1))
                {
                    return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
                }
                return Encoding.GetEncoding("GBK").GetString(stream.ToArray());
            }
        }

        public string GetUtf8(string Url)
        {
            using (MemoryStream stream = this.GetHtml(Url, null, null))
            {
                return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
            }
        }

        public string PostUtf8(string Url, string postData, StringBuilder Header = null)
        {
            using (MemoryStream stream = this.GetHtml(Url, Encoding.UTF8.GetBytes(postData), Header))
            {
                return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
            }
        }

        public Image GetImage(string Url)
        {
            using (MemoryStream stream = this.GetHtml(Url, null, null))
            {
                if (stream == null)
                {
                    return null;
                }
                return Image.FromStream(stream);
            }
        }
        #endregion

        public Image GetImage(MemoryStream mstream)
        {
            using (MemoryStream stream = mstream)
            {
                if (stream == null)
                {
                    return null;
                }
                return Image.FromStream(stream);
            }
        }

        /// <summary>
        /// 可以自定义 UserAgent访问
        /// </summary>
        /// <param name="mstream"></param>
        /// <returns></returns>
        public string GetDataPro(MemoryStream mstream)
        {
            using (MemoryStream stream = mstream)
            {
                if (stream != null)
                {
                    Match match = Regex.Match(Encoding.Default.GetString(stream.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    string str = (match.Groups.Count > 1) ? match.Groups[2].Value.ToUpper().Trim() : string.Empty;
                    if (str.IndexOf("\"") > 0)
                    {
                        str = str.Split(new char[] { '"' })[0];
                    }
                    if ((str.Length > 2) && (str.IndexOf("UTF-8") != -1))
                    {
                        return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
                    }
                    return Encoding.GetEncoding("GBK").GetString(stream.ToArray());
                }
                return null;
            }
        }

        public string GetDataPro(MemoryStream mstream, Encoding pEncoding)
        {
            using (MemoryStream stream = mstream)
            {
                if (stream != null)
                {
                    return pEncoding.GetString(stream.ToArray());
                }
                return null;
            }
        }

        /// <summary>
        /// 可以自定义 UserAgent访问 不能获取cookie
        /// </summary>
        public MemoryStream GetHtmlPro(string Url, byte[] Postdata, string proxy = null, string subcookie = null, string subheader = null, bool isMoved = true)//true自动重定向
        {
            MemoryStream stream2;
            try
            {
                Thread thread;
                ThreadStart start = null;
                ThreadStart start2 = null;

                Uri uri = new Uri(Url);
                string getMethod = "GET";
                //if (string.IsNullOrEmpty(Postdata))
                //{
                //    getMethod = "GET";
                //}
                //else
                //{
                //    getMethod = "POST";
                //}
                if (Postdata.Length == 0)
                {
                    getMethod = "GET";
                }
                else
                {
                    getMethod = "POST";
                }
                bool flag = Url.Substring(0, 5) == "https";
                int ulSession = 0;
                if (string.IsNullOrEmpty(proxy))
                {
                    ulSession = InternetOpen(this.UserAgent, 1, string.Empty, string.Empty, 0);
                }
                else if (flag)
                {
                    ulSession = InternetOpen(this.UserAgent, 3, proxy, string.Empty, 0);
                }
                else
                {
                    ulSession = InternetOpen(this.UserAgent, 3, "http=" + proxy, string.Empty, 0);
                }
                if (ulSession == 0)
                {
                    InternetCloseHandle(ulSession);
                    return null;
                }
                int netConet = InternetConnect(ulSession, uri.Host, uri.Port, string.Empty, string.Empty, 3, 0, 0);
                if (netConet == 0)
                {
                    InternetCloseHandle(netConet);
                    InternetCloseHandle(ulSession);
                    return null;
                }
                /*
                DWORD dwOpenRequestFlags = INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP | // ex: https:// to http:// 0x00008000
		            INTERNET_FLAG_KEEP_CONNECTION | //// use keep-alive semantics KEEP_CONNECTION 0x00400000
		            INTERNET_FLAG_NO_AUTH |    //不试图自动验证。  0x00040000
		            INTERNET_FLAG_NO_COOKIES | //不会自动添加的Cookie头到请求，并且不自动添加返回的cookie到cookie数据库。 0x00080000
		            INTERNET_FLAG_NO_UI |      //禁用Cookie的对话框。 0x00000200
		            //设置启用HTTPS
		            INTERNET_FLAG_SECURE | //https 0x00800000
		            INTERNET_FLAG_RELOAD;  //从原服务器强制下载所要求的文件，对象，或目录列表，而不是从缓存下载。 0x80000000
                    
                
                INTERNET_FLAG_NEED_FILE //如果要创建的文件不能被缓存，创建临时文件。 0x00000010
                INTERNET_FLAG_PRAGMA_NOCACHE    0x00000100  // asking wininet to add "pragma: no-cache"  & 0xfffffeff
                */
                uint num3 = 0x80000000 | 0x00000010 | 0x00008000 | 0x00400000 | 0x00040000 | 0x00000100;
                //if (!string.IsNullOrEmpty(subcookie))
                {
                    num3 |= 0x00080000 | 0x00000200;//INTERNET_FLAG_NO_COOKIES | INTERNET_FLAG_NO_UI
                }
                if (!isMoved)
                {
                    num3 |= 0x200000;//不自动重定向INTERNET_FLAG_NO_AUTO_REDIRECT
                }
                if (flag)
                {
                    num3 |= 0x800000;//INTERNET_FLAG_SECURE  https PCT/SSL if applicable (HTTP)
                }
                else
                {
                    num3 |= 0x4000;//INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTPS  ex: http:// to https://
                }
                int httpRequest = HttpOpenRequest(netConet, getMethod, uri.PathAndQuery, "HTTP/1.1", string.Empty, string.Empty, (int)num3, 0);
                if (httpRequest == 0)
                {
                    InternetCloseHandle(httpRequest);
                    InternetCloseHandle(netConet);
                    InternetCloseHandle(ulSession);
                    return null;
                }

                string strHeader = string.Empty;
                if (string.IsNullOrEmpty(subheader))
                {
                    strHeader = ((strHeader + "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n")
                        + "Content-Type:application/x-www-form-urlencoded\r\n"
                        + "Accept-Language:zh-cn\r\n")
                        + "Referer:" + Url;
                }
                else
                {
                    strHeader = subheader;
                }
                if (string.Equals(getMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {

                    if (this._WininetTimeOut > 0)
                    {
                        if (start == null)
                        {
                            start = delegate
                            {
                                HttpSendRequestA(httpRequest, strHeader.ToString(), strHeader.Length, null, 0);//get
                            };
                        }
                        thread = new Thread(start);
                        thread.IsBackground = true;
                        thread.Start();
                        thread.Join(this._WininetTimeOut);
                    }
                    else
                    {
                        HttpSendRequestA(httpRequest, strHeader.ToString(), strHeader.Length, null, 0);//get
                    }
                }
                else
                {
                    if (this._WininetTimeOut > 0)//post
                    {
                        if (start2 == null)
                        {
                            start2 = delegate
                            {
                                HttpSendRequestA(httpRequest, strHeader.ToString(), strHeader.Length, Postdata, Postdata.Length);//post
                            };
                        }
                        thread = new Thread(start2);
                        thread.IsBackground = true;
                        thread.Start();
                        thread.Join(this._WininetTimeOut);
                    }
                    else
                    {
                        HttpSendRequestA(httpRequest, strHeader.ToString(), strHeader.Length, Postdata, Postdata.Length);//post
                    }
                }


                int revSize = 0;

                MemoryStream stream = new MemoryStream();



                int num3s = 0;
                byte[] array = new byte[1024];
                MemoryStream memoryStream = new MemoryStream();
                while (true)
                {
                    bool flags = InternetReadFile(httpRequest, array, 1024, out num3s);
                    if (!flags || num3s <= 0)
                    {
                        break;
                    }
                    memoryStream.Write(array, 0, num3s);
                }
                InternetCloseHandle(httpRequest);
                InternetCloseHandle(netConet);// num2
                InternetCloseHandle(ulSession);
                return memoryStream;

                #region 查询响应头信息
                //HTTP_QUERY_SET_COOKIE 接收数值为请求设置的cookie。 43
                //HTTP_QUERY_RAW_HEADERS_CRLF 接收所有由服务器返回的HEAD。 22

                bool flagInfo = false;
                try
                {
                    HttpEndRequest(httpRequest, null, 0, 0);
                    byte[] bufferInfo = new byte[65536];
                    flagInfo = HttpQueryInfo(httpRequest, 22, bufferInfo, 65536, revSize);
                    if (flagInfo)
                    {
                        stream.Write(bufferInfo, 0, revSize);
                        stream.WriteByte(0);
                    }
                    bufferInfo = null;
                }
                catch (Exception)
                {
                }
                #endregion

                revSize = 0;
                byte[] pByte = new byte[0x400];
                while (true)
                {
                    if (InternetReadFile(httpRequest, pByte, 0x400, out revSize) && (revSize > 0))
                    {
                        stream.Write(pByte, 0, revSize);
                    }
                    else
                    {
                        goto goout;
                    }
                }
                goout:

                InternetCloseHandle(httpRequest);
                InternetCloseHandle(netConet);
                InternetCloseHandle(ulSession);
                stream2 = stream;

            }
            catch (Exception)
            {
                throw;
            }
            return stream2;
        }





        public DateTime GetLocalTime()
        {
            SystemTime sysTime = new SystemTime();
            GetLocalTime(ref sysTime);
            return new DateTime(sysTime.wYear, sysTime.wMonth, sysTime.wDay, sysTime.wHour, sysTime.wMinute, sysTime.wSecond);
        }

        public void SetLocalTime(DateTime dt)
        {
            SystemTime sysTime = new SystemTime
            {
                wYear = (ushort)dt.Year,
                wMonth = (ushort)dt.Month,
                wDay = (ushort)dt.Day,
                wHour = (ushort)dt.Hour,
                wMinute = (ushort)dt.Minute,
                wSecond = (ushort)dt.Second
            };
            SetLocalTime(ref sysTime);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool HttpAddRequestHeaders(int hRequest, string szHeasers, uint headersLen, uint modifiers);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int HttpOpenRequest(int hConnect, string szVerb, string szURI, string szHttpVersion, string szReferer, string accetpType, int dwflags, int dwcontext);
        [DllImport("wininet.dll")]
        private static extern bool HttpSendRequestA(int hRequest, string szHeaders, int headersLen, byte[] options, int optionsLen);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetCloseHandle(int ulSession);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetConnect(int ulSession, string strServer, int ulPort, string strUser, string strPassword, int ulService, int ulFlags, int ulContext);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetOpen(string strAppName, int ulAccessType, string strProxy, string strProxyBypass, int ulFlags);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetReadFile(int hRequest, byte[] pByte, int size, out int revSize);

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetReadFileExA(int hRequest, byte[] pByte, int size, out int revSize);


        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string url, string name, StringBuilder data, ref int dataSize);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        //http://blog.csdn.net/hutao1101175783/article/details/42589783
        //HTTP_QUERY_SET_COOKIE 接收数值为请求设置的cookie。 43
        //HTTP_QUERY_RAW_HEADERS_CRLF 接收所有由服务器返回的HEAD。 22
        //#define HTTP_QUERY_RAW_HEADERS_CRLF             22  // special: all headers
        //读取返回的cookie  
        //flag = HttpQueryInfoA(hRequest, HTTP_QUERY_SET_COOKIE, lpOutBuffer, &dwSize, NULL);//flag true success  flag := HttpQueryInfo(hUrl, HTTP_QUERY_RAW_HEADERS_CRLF, buffer, 65536, 0);
        //printf("%s\n", lpOutBuffer);

        [DllImport("wininet.dll")]
        private static extern bool HttpQueryInfo(int ulSession, int dwInfoLevel, byte[] pByte, int lpdwBufferLength, int revSize);
        [DllImport("wininet.dll")]
        private static extern bool HttpEndRequest(int ulSession, string pByte, int lpdwBufferLength, int dwContext);


        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime sysTime);

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }
}
