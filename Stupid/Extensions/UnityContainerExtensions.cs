using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Extensions
{

    /// <summary>
    /// UnityContainer拓展
    /// </summary>
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// 注册程序集类别
        /// </summary>
        /// <param name="container">注册中心</param>
        /// <param name="assembly">程序集</param>
        /// <param name="baseType">类别</param>
        public static void RegisterInheritedTypes(this IUnityContainer container, Assembly assembly, Type baseType)
        {
            var allTypes = assembly.GetTypes();
            var baseInterfaces = baseType.GetInterfaces();

            foreach (var type in allTypes)
            {
                if (type.BaseType != null && type.BaseType.GenericEq(baseType))
                {
                    var typeInterface = type.GetInterfaces().FirstOrDefault(p => !baseInterfaces.Any(f => f.GenericEq(p)));
                    if (typeInterface == null)
                        continue;
                    container.RegisterType(typeInterface, type);
                }
            }
        }
    }
}
