using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 非契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotSpecification<T> : SpecificationBase<T>
    {
        private ISpecification<T> _ispecification;

        /// <summary>
        /// 非契约
        /// </summary>
        /// <param name="specification"></param>
        public NotSpecification(ISpecification<T> specification)
        {
            this._ispecification = specification;
        }

        /// <summary>
        /// 获取lambda
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            var body = Expression.Not(this._ispecification.GetExpression().Body);

            return Expression.Lambda<Func<T, bool>>(body, this._ispecification.GetExpression().Parameters);
        }
    }
}
