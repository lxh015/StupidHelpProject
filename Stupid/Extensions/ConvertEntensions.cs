using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{
    /// <summary>
    /// 转换拓展帮助类
    /// </summary>
    public static class ConvertEntensions
    {
        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="obj">要转换的值</param>
        /// <returns></returns>
        public static T ConvertType<T>(this object obj)
        {
            if (obj == null)//返回类型的默认值
                goto error;

            Type tp = typeof(T);

            if (tp.IsGenericType)
                tp = tp.GetGenericArguments()[0];

            //string直接返回转换
            if (tp.Name.ToLower() == "string")
                return (T)obj;

            //反射获取TryParse方法
            var TryParse = tp.GetMethod("TryParse", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, Type.DefaultBinder,
                                        new Type[] { typeof(string), tp.MakeByRefType() },
                                        new ParameterModifier[] { new ParameterModifier(2) });

            var parameters = new object[] { obj, Activator.CreateInstance(tp) };
            bool success = (bool)TryParse.Invoke(null, parameters);

            //成功返回转换后的值，否则返回类型的默认值
            if (success)
                return (T)parameters[1];


            error:
            return default(T);
        }

        /// <summary>
        /// 将字符串转换为long类型ip地址
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long StringToIpAddress(this string value)
        {
            char[] separator = new char[] { '.' };
            string[] items = value.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        /// <summary>
        /// 将Long数据转换为string字符串IP地址
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LongToIpAddress(this long value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((value >> 24) & 0xFF).Append(".");
            sb.Append((value >> 16) & 0xFF).Append(".");
            sb.Append((value >> 8) & 0xFF).Append(".");
            sb.Append(value & 0xFF);
            return sb.ToString();
        }
    }
}
