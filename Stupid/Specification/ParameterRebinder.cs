using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 代表的参数重新绑定用于重新绑定参数给定的表达式。这是解决方案的一部分,解决了参数的表达式问题时要使用Apworks实体框架规范
    /// </summary>
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this._map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 分配参数
        /// </summary>
        /// <param name="map"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// 检查lambda参数
        /// </summary>
        /// <param name="pexp"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression pexp)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(pexp, out replacement))
            {
                pexp = replacement;
            }

            return base.VisitParameter(pexp);
        }
    }
}
