using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 表达式契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ExpressionSpecification<T> : SpecificationBase<T>
    {
        private Expression<Func<T, bool>> expression;

        /// <summary>
        /// 表达式契约
        /// </summary>
        /// <param name="expression"></param>
        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// 获取lambda数据
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return this.expression;
        }
    }
}
