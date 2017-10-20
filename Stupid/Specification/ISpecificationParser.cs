using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCriteria"></typeparam>
    public interface ISpecificationParser<TCriteria>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="specification"></param>
        /// <returns></returns>
        TCriteria Parse<T>(ISpecification<T> specification);
    }
}
