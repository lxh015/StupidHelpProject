using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Stupid.WebApiSafe
{
    /// <summary>
    /// WebApi验证
    /// </summary>
    public class WebApiTokenAttribute:AuthorizeAttribute
    {
        /// <summary>
        /// 重载验证方法
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            WebApiAboutToken token = new WebApiAboutToken();
            string message = string.Empty;
            var result = token.TokenCheck(filterContext.HttpContext, out message);

            if (!result)
            {
                var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                var SJR = new StanderJsonResult();
                SJR.Result = message;
                SJR.Encoding = filterContext.HttpContext.Request.ContentEncoding;
                SJR.ExecuteResult(filterContext);
                filterContext.Result = SJR;
            }
            return;
        }
        
        /// <summary>
        /// 显示相关错误信息
        /// </summary>
        public class StanderJsonResult : ActionResult
        {
            /// <summary>
            /// 错误原因
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// Http内容类型
            /// </summary>
            public string ContentType { get; set; }

            /// <summary>
            /// Http编码
            /// </summary>
            public System.Text.Encoding Encoding { get; set; }

            /// <summary>
            /// 将错误原因加入到HttpContext中。
            /// </summary>
            /// <param name="context"></param>
            public override void ExecuteResult(ControllerContext context)
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = string.IsNullOrEmpty(ContentType) ?
                    "application/json" : ContentType;

                if (Encoding != null)
                {
                    response.ContentEncoding = Encoding;
                }
                var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                string result = json.Serialize(Result);
                response.Write(result);
            }
        }
    }
}
