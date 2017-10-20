using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 契约基础
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        /// <summary>
        /// 获取Expression表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static SpecificationBase<T> Eval(Expression<Func<T, bool>> expression)
        {
            return new ExpressionSpecification<T>(expression);
        }

        /// <summary>
        /// 获取一个<see cref="System.Boolean"/>值表示是否符合规约
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool IsSatisfiedBy(T obj)
        {
            return this.GetExpression().Compile()(obj);
        }

        /// <summary>
        /// 结合当前规约实例与另一个规约实例并返回组合规约代表当前和给定的规约必须满足给定对象。
        /// </summary>
        /// <param name="other">另一个规约</param>
        /// <returns></returns>
        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSepcification<T>(this, other);
        }

        /// <summary>
        /// 结合当前规范实例与另一个规范实例并返回组合规范代表当前或给定的规范应该满足给定的对象。
        /// </summary>
        /// <param name="other">另一个规约</param>
        /// <returns></returns>
        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new OrSepcification<T>(this, other);
        }

        /// <summary>
        /// 结合当前规范实例与另一个规范实例并返回组合规范代表当前规范应满足给定对象,但不应该指定规范。
        /// </summary>
        /// <param name="other">另一个规约</param>
        /// <returns></returns>
        public ISpecification<T> AndNot(ISpecification<T> other)
        {
            return new AndNotSepcification<T>(this, other);
        }

        /// <summary>
        /// 结合当前规范实例与另一个规范实例并返回组合规范代表当前规范应满足给定对象。
        /// </summary>
        /// <returns></returns>
        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
        /// <summary>
        /// 获取Linq表达式
        /// </summary>
        /// <returns>The LINQ expression.</returns>
        public abstract Expression<Func<T, bool>> GetExpression();
    }
}
