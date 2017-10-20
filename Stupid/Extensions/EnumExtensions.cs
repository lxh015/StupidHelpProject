using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{
    /// <summary>
    /// Enum拓展
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取Enum中对其描述的信息。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum e)
        {
            try
            {
                Type type = e.GetType();
                var attributes = type.GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (attributes.Length == 0) return null;
                return attributes[0].Description;
            }
            catch
            {
                return null;
            }
        }
    }
}
