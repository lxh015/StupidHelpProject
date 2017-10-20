using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stupid.SomeConvert
{
    /// <summary>
    /// 将Xml文档转换成泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlToEntity<T> where T : class
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="isWebApi">是否是WebApi</param>
        /// <returns></returns>
        public static T ToEntity(string xml, bool isWebApi = false)
        {
            try
            {
                //防止出现一个数据中程序自动添加没用信息的错误
                if (isWebApi)
                {
                    if (xml.Contains("xmlns"))
                    {
                        int start = xml.IndexOf("xmlns");
                        int end = xml.IndexOf("http://tempuri.org/") + 20;
                        int count = end - start;
                        xml = xml.Remove(start, count);
                    }
                }

                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    object obj = xmldes.Deserialize(sr);
                    return (T)obj;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 泛型转Xml
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToXml(T source)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, source);
            return sw.ToString();
        }
    }
}
