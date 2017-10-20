using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 空契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class NoneSpecification<T> : SpecificationBase<T>
    {
        /// <summary>
        /// 获取lambda
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return o => true;
        }
    }
}
