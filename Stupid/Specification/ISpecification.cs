using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// Specification contract.
    /// 具体需要具体了解关于规约的内容请参见 Martin Fowler 的文章：http://martinfowler.com/apsupp/spec.pdf.
    /// By Evan.
    /// </summary>
    /// <typeparam name="T">应用规约的对象类型</typeparam>
    public interface ISpecification<T>
    {
        /// <summary> 
        /// 验证对象是否满足规约.
        /// </summary>
        /// <param name="obj">对象实体</param>
        /// <returns>是否满足</returns>
        bool IsSatisfiedBy(T obj);

        /// <summary>
        /// 以And方式组合当前规约与参数给定的规约,返回组合后的规约.
        /// 对于组合后的规约只有在前面的2者都满足的情况下才满足.
        /// </summary>
        /// <param name="other">要被组合的规约</param>
        /// <returns>返回组合后的规约</returns>
        ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// 以Or方式组合当前规约与参数给定的规约,返回组合后的规约.
        /// 对于组合后的规约只要在前面的2者都有一个满足的情况下就满足.
        /// </summary>
        /// <param name="other">要被组合的规约</param>
        /// <returns>返回组合后的规约</returns>
        ISpecification<T> Or(ISpecification<T> other);

        /// <summary>
        /// 以AndNot方式组合当前规约与参数给定的规约,返回组合后的规约.
        /// 对于组合后的规约对象满足当前规约但是不满足给定的规约情况下满足(True).
        /// </summary>
        /// <param name="other">要被组合的规约</param>
        /// <returns>返回组合后的规约</returns>
        ISpecification<T> AndNot(ISpecification<T> other);

        /// <summary>
        /// 以Not方式组合当前规约与参数给定的规约,返回组合后的规约.
        /// 返回当前规约相反的规约.
        /// </summary>
        /// <returns>返回当前规约相反的规约</returns>
        ISpecification<T> Not();

        /// <summary>
        /// 返回当前规约的Linq表达式.
        /// </summary>
        /// <returns>Linq表达式</returns>
        Expression<Func<T, bool>> GetExpression();
    }
}
