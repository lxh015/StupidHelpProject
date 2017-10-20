using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 或契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrSepcification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// 或契约
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public OrSepcification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        /// <summary>
        /// 获取拉姆达表达式
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return Left.GetExpression().Or(Right.GetExpression());
        }
    }
}
