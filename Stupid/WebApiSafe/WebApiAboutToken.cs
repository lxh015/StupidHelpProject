using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Stupid.WebApiSafe
{
    /// <summary>
    /// Token相关
    /// </summary>
    public class WebApiAboutToken
    {
        /// <summary>
        /// 客户端头部关键字。
        /// </summary>
        public enum TokenHeadKeys
        {
            /// <summary>
            /// 随机数
            /// </summary>
            [Description("随机数")]
            random = 1,
            /// <summary>
            /// 客户端时间
            /// </summary>
            [Description("客户端时间")]
            spantime = 2,
            /// <summary>
            /// 摘文
            /// </summary>
            [Description("摘文")]
            pickstr = 3,
        }

        /// <summary>
        /// 8位Des加密密钥。
        /// </summary>
        protected const string DesKey = "J8_3m/2?";


        #region 服务端操作

        /// <summary>
        /// 建立基础密钥。
        /// </summary>
        /// <param name="admin">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public string CreateDesToken(string admin, string password)
        {
            return Stupid.Common.EncipherCommon.EnDes((admin + password), DesKey);
        }


        /// <summary>
        /// 服务端检查Token。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool TokenCheck(HttpContextBase context, out string errorMsg)
        {
            errorMsg = string.Empty;

            if (!IsService())
            {
                errorMsg = "客户端不能使用该方法！";
                return false;
            }

            try
            {
                var spantime = context.Request.Headers.Get(TokenHeadKeys.spantime.ToString());

                try
                {
                    var STime = Convert.ToDateTime(spantime);
                    var NTime = DateTime.Now;
                    var timeCut = NTime - STime;

                    if (timeCut.Days > 0 || timeCut.TotalMinutes >= 1)
                    {
                        errorMsg = "链接超时！";
                        return false;
                    }
                }
                catch
                {
                    errorMsg = "时间戳错误！";
                    return false;
                }

                var random = context.Request.Headers.Get(TokenHeadKeys.random.ToString());
                var pickstr = context.Request.Headers.Get(TokenHeadKeys.pickstr.ToString());

                #region 空值判断
                if (string.IsNullOrEmpty(random))
                {
                    errorMsg = "随机数错误！";
                    return false;
                }

                if (string.IsNullOrEmpty(spantime))
                {
                    errorMsg = "时间戳错误！";
                    return false;
                }

                if (string.IsNullOrEmpty(pickstr))
                {
                    errorMsg = "摘文错误！";
                    return false;
                }
                #endregion

                //位移量。
                int shift = 0;
                //实际随机数
                int rand = 0;

                //随机数长度限制。5位以上7位以下。
                if (random.Length < 5 || random.Length != 5)
                {
                    errorMsg = "随机数错误！";
                    return false;
                }

                SetRandomAndShift(random, out rand, out shift);
                if (rand == 0)
                {
                    errorMsg = "随机数错误！";
                    return false;
                }

                if (pickstr.Length != 5)
                {
                    errorMsg = "摘文错误！";
                    return false;
                }

                //获取客户端IP地址。以便寻找服务端缓存。
                var clientip = context.Request.UserHostAddress;
                var baseKey = TokenCache.GetToken(context, clientip);

                if (string.IsNullOrEmpty(baseKey))
                {
                    errorMsg = "基础信息错误！";
                    return false;
                }


                var full = CreateFullKey(baseKey, random);

                if (!IsTruePick(pickstr, full, shift))
                {
                    errorMsg = "摘文错误！";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 建立完整密钥。
        /// </summary>
        /// <param name="baseKey">基础密钥</param>
        /// <param name="random">客户端的随机数</param>
        /// <returns></returns>
        public string CreateFullKey(string baseKey, string random)
        {
            return Stupid.Common.EncipherCommon.MD5((random + baseKey));
        }

        /// <summary>
        /// 将随机数拆分，获取真·随机数和位移码。
        /// </summary>
        /// <param name="random"></param>
        /// <param name="rand"></param>
        /// <param name="shift"></param>
        protected void SetRandomAndShift(string random, out int rand, out int shift)
        {
            rand = 0;
            shift = 0;
            try
            {
                var randstr = random.Substring(0, 4);
                var shiftstr = random.Substring(4);

                rand = Convert.ToInt32(randstr);
                shift = Convert.ToInt32(shiftstr);
            }
            catch
            { }
        }

        /// <summary>
        /// 检查摘文。
        /// </summary>
        /// <param name="pickstr">客户端摘文</param>
        /// <param name="fullKey">完整Key</param>
        /// <param name="shift">位移量</param>
        /// <returns></returns>
        protected bool IsTruePick(string pickstr, string fullKey, int shift)
        {
            try
            {
                var isContain = fullKey.Contains(pickstr);
                if (isContain)
                {
                    var pickLenght = pickstr.Length;
                    var copy = fullKey.Substring(shift, pickLenght);

                    if (copy != pickstr)
                        return false;

                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是服务端
        /// </summary>
        /// <returns></returns>
        public static bool IsService()
        {
            #region 服务端判定
            try
            {
                var serviceMaker = System.Configuration.ConfigurationManager.AppSettings["IsService"];
                if (string.IsNullOrEmpty(serviceMaker))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            #endregion
        }

        #endregion

        #region 客户端操作

        /// <summary>
        /// 建立客户端Key。
        /// </summary>
        /// <param name="baseKey"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public string CreateClientKey(string baseKey, int random)
        {

            var full = this.CreateFullKey(baseKey, random.ToString());


            var shift = random.ToString().Substring(4);
            var takeLength = 5;
            var start = Convert.ToInt32(shift);
            var result = full.Substring(start, takeLength);

            return result.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Token缓存操作
    /// </summary>
    public class TokenCache
    {
        /// <summary>
        /// 获取Cache中的缓存基础Key
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetToken(HttpContextBase context, string ip)
        {
            if (!WebApiAboutToken.IsService())
            {
                return string.Empty;
            }

            try
            {
                var token = context.Cache.Get(ip).ToString();

                return token;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 添加基础Key
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ip"></param>
        /// <param name="key"></param>
        public static void InsertToken(HttpContextBase context, string ip, string key)
        {
            if (!WebApiAboutToken.IsService())
            {
                return;
            }

            context.Cache.Insert(ip, key);
        }
    }
}
