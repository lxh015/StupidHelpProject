using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 任意条件契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AnySpecification<T> : SpecificationBase<T>
    {
        /// <summary>
        /// 获取拉姆达表达式
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return o => true;
        }
    }
}
