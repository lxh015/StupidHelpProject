using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Stupid.Extensions;

namespace Stupid.Ioc
{
    /// <summary>
    /// 依赖注入容器
    /// 
    /// <!--使用必须在Global中添加以下代码-->
    /// <code>Ioc.RegisterInheritedTypes(typeof(xxxx.ServiceBase).Assembly, typeof(xxx.ServiceBase));</code>
    /// <code>
    ///  //EntityFramework预热
    ///  <!--using (var dbcontext = new EntityFramework.DefaultDbContext())
    ///       {
    ///            var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
    ///  var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
    ///  mappingCollection.GenerateViews(new List<EdmSchemaError>());
    ///         }-->
    /// </code>
    /// </summary>
    public class Ioc
    {
        private static readonly UnityContainer container;

        static Ioc()
        {
            container = new UnityContainer();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImpmentation"></typeparam>
        public static void Register<TInterface, TImpmentation>() where TImpmentation : TInterface
        {
            container.RegisterType<TInterface, TImpmentation>();
        }

        /// <summary>
        /// 注册基础信息
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="baseType">基类别</param>
        public static void RegisterInheritedTypes(Assembly assembly, Type baseType)
        {
            container.RegisterInheritedTypes(assembly, baseType);
        }

        /// <summary>
        /// 获取注册类别中的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return container.Resolve<T>();
        }
    }
}
