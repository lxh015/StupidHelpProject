using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{
    /// <summary>
    /// Encoding拓展帮助类
    /// </summary>
    public static class EncodingExtensions
    {
        /// <summary>
        /// 通过将Object数据转换成Json数据来进行数据压缩。
        /// </summary>
        /// <param name="encod"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Encoding encod,object obj)
        {
            var Json =new System.Web.Script.Serialization.JavaScriptSerializer();
            var toJson = Json.Serialize(obj);

            byte[] inputBytes = System.Text.Encoding.Default.GetBytes(toJson);

            return inputBytes;
        }
    }
}
