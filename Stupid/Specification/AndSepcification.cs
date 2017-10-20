using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 和契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AndSepcification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// 和契约
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public AndSepcification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        /// <summary>
        /// 获取lambda数据
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return Left.GetExpression().And(Right.GetExpression());
        }
    }
}
