using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Stupid
{
    /// <summary>
    /// 登陆验证 By Stupid
    /// 若需使用请在AppSetting中添加一个名称为StupidKey的值
    /// </summary>
    public class SLoginAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 用于登陆后信息加密的密钥
        /// </summary>
        protected const string LoginedKey = ")d~39+jdn<jdn$_dj*7-77";

        /// <summary>
        /// 逻辑设置
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            base.AuthorizeCore(httpContext);

            try
            {
                string loginname = System.Configuration.ConfigurationManager.AppSettings["BlogKey"].ToString();
                var info = httpContext.Cache[loginname];
                if (info == null)
                {
                    httpContext.Response.StatusCode = 848;
                    return false;
                }

                var oldinfo = Stupid.Common.EncipherCommon.DeDes(info.ToString(), LoginedKey);

                #region 进行一些验证

                var oldinfosp = oldinfo.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (oldinfosp.Length != 3)
                {
                    httpContext.Response.StatusCode = 848;
                    return false;
                }

                try
                {
                    DateTime first = Convert.ToDateTime(oldinfosp[0]);
                }
                catch
                {
                    httpContext.Response.StatusCode = 848;
                    return false;
                }

                try
                {
                    int second = Convert.ToInt32(oldinfosp[1]);
                }
                catch
                {
                    httpContext.Response.StatusCode = 848;
                    return false;
                }

                try
                {
                    var threesp = oldinfosp[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (threesp.Length != 3)
                    {
                        httpContext.Response.StatusCode = 848;
                        return false;
                    }

                    var threeinfo = Stupid.Common.EncipherCommon.DeDes(threesp[2], "");
                    var key = threeinfo.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);



                    if (!key[1].Equals(threesp[0]))
                    {
                        httpContext.Response.StatusCode = 848;
                        return false;
                    }

                    var pwd = Stupid.Common.EncipherCommon.EnDes(key[2], "");

                    if (!pwd.Equals(threesp[1]))
                    {
                        httpContext.Response.StatusCode = 848;
                        return false;
                    }
                }
                catch
                {
                    httpContext.Response.StatusCode = 848;
                    return false;
                }
                #endregion
            }
            catch
            {
                if (httpContext.Response.StatusCode == 302)
                {

                }
                else
                {
                    httpContext.Response.StatusCode = 848;
                }
                return false;
            }


            return true;
        }
        
        /// <summary>
        /// 执行页面跳转
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //该行代码为解决设置848后与Result中的Code冲突
            //所导致的一个：服务器无法在已发送 HTTP 标头之后设置状态。异常
            filterContext.Result = null;

            if (filterContext.HttpContext.Response.StatusCode == 848)
            {
                var nowurl = filterContext.HttpContext.Request.RawUrl.ToString();

                filterContext.HttpContext.Response.Redirect("/Login/Index?ReturnUrl=" + nowurl);
            }
        }
    }
}
