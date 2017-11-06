using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{
    /// <summary>
    /// CookieContainer 拓展
    /// </summary>
    public static class CookieExpand
    {
        /// <summary>
        /// 将CookieContainer中的cookie数据取出
        /// </summary>
        /// <param name="cc">CookieContainer数据</param>
        /// <returns></returns>
        public static List<Cookie> ConvertToCookieList(this CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
            System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }

            return lstCookies;
        }
    }
}
