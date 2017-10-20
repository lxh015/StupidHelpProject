using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.SomeConvert
{
    /// <summary>
    /// 将DataTable转换成T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataToList<T> where T : class, new()
    {
        /// <summary>
        /// DataTable转换成List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToModel(DataTable dt)
        {
            IList<T> ts = new List<T>();

            // 获得此模型的类型
            System.Type type = typeof(T);
            string tempName = "";

            foreach (DataRow item in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性
                System.Reflection.PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (System.Reflection.PropertyInfo pi in propertys)
                {
                    // 检查DataTable是否包含此列
                    tempName = pi.Name;

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Set
                        if (!pi.CanWrite)
                            continue;

                        object value = item[tempName];


                        //将Double转成Single
                        if (pi.PropertyType == typeof(Single))
                        {
                            value = Convert.ToSingle(value);
                        }

                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }

            return ts;
        }
    }
}
