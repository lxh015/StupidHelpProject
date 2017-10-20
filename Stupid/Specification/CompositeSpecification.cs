using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 复合契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CompositeSpecification<T> : SpecificationBase<T>, ICompositeSpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        /// <summary>
        /// 复合契约
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public CompositeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this._left = left;
            this._right = right;
        }

        /// <summary>
        /// 左侧lambda表达式
        /// </summary>
        public ISpecification<T> Left
        {
            get { return this._left; }
        }

        /// <summary>
        /// 右侧lambda表达式
        /// </summary>
        public ISpecification<T> Right
        {
            get { return this._right; }
        }
    }
}
