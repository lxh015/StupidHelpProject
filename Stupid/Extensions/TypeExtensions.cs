using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{
    /// <summary>
    /// 类别拓展
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断类别与基类别是否相同
        /// </summary>
        /// <param name="type">基类别</param>
        /// <param name="toCompare">要对比类别</param>
        /// <returns></returns>
        public static bool GenericEq(this Type type, Type toCompare)
        {
            return type.Namespace == toCompare.Namespace && type.Name == toCompare.Name;
        }
    }
}
