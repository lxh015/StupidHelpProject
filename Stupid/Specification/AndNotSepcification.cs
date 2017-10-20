using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 且不契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AndNotSepcification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// 且不契约
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public AndNotSepcification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        /// <summary>
        /// 获取lambda数据
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            var bodyNot = Expression.Not(Right.GetExpression().Body);

            var bodyNotExpression = Expression.Lambda<Func<T, bool>>(bodyNot, Right.GetExpression().Parameters);

            return Left.GetExpression().And(bodyNotExpression);
        }
    }
}
