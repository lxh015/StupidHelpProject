using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 实现类复合规范。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompositeSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// 左侧lambda
        /// </summary>
        ISpecification<T> Left { get; }

        /// <summary>
        /// 右侧lambda
        /// </summary>
        ISpecification<T> Right { get; }
    }
}
