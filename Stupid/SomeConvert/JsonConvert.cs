using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Stupid.SomeConvert
{
    /// <summary>
    /// 序列化。不好用
    /// </summary>
    [Obsolete("不好用，可以使用Newtonsoft.Json")]
    public class JsonConvert
    {
        /// <summary>
        /// 代码解析Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ConvertToEntity<T>(string str)
        {
            Type t = typeof(T);
            object obj = Activator.CreateInstance(t);
            var properties = t.GetProperties();
            string m = str.Trim('{').Trim('}');
            string[] arr = m.Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                for (int k = 0; k < properties.Count(); k++)
                {
                    int hasTitle = 0;
                    string Name = "";
                    if (arr[i].Split(':').Length >= 2)
                    {
                        hasTitle = 1;
                        Name = arr[i].Substring(arr[i].IndexOf("{") + 2);
                        Name = Name.Substring(0, Name.IndexOf("\""));
                    }
                    else
                        Name = arr[i].Substring(0, arr[i].IndexOf(":"));
                    object Value = null;
                    if (hasTitle == 1)
                    {
                        string first = arr[i].Substring(arr[i].IndexOf(":") + 1);
                        Value = first.Substring(first.IndexOf(":") + 1);
                    }
                    else
                        Value = arr[i].Substring(arr[i].IndexOf(":") + 1);
                    if (properties[k].Name.Equals(Name))
                    {
                        if (properties[k].PropertyType.Equals(typeof(int)))
                        {
                            properties[k].SetValue(obj, Convert.ToInt32(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(string)))
                        {
                            properties[k].SetValue(obj, Convert.ToString(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(long)))
                        {
                            properties[k].SetValue(obj, Convert.ToInt64(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(decimal)))
                        {
                            properties[k].SetValue(obj, Convert.ToDecimal(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(double)))
                        {
                            properties[k].SetValue(obj, Convert.ToDouble(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(Nullable<int>)))
                        {
                            properties[k].SetValue(obj, Convert.ToInt32(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(Nullable<decimal>)))
                        {
                            properties[k].SetValue(obj, Convert.ToDecimal(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(Nullable<long>)))
                        {
                            properties[k].SetValue(obj, Convert.ToInt64(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(Nullable<double>)))
                        {
                            properties[k].SetValue(obj, Convert.ToDouble(Value), null);
                        }
                        if (properties[k].PropertyType.Equals(typeof(Nullable<DateTime>)))
                        {
                            properties[k].SetValue(obj, Convert.ToDateTime(Value), null);
                        }

                    }
                }

            }
            return (T)obj;
        }

        /// <summary>
        /// 将泛型转换为String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static string T2Json<T>(T Entity)
        {
            var json = new JavaScriptSerializer();
            string result = json.Serialize(Entity);
            return result;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {

            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式

            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";

            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);

            Regex reg = new Regex(p);

            jsonString = reg.Replace(jsonString, matchEvaluator);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {

            string result = string.Empty;

            DateTime dt = DateTime.Parse(m.Groups[0].Value);

            dt = dt.ToUniversalTime();

            TimeSpan ts = dt - DateTime.Parse("1970-01-01");

            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);

            return result;

        }
    }
}
