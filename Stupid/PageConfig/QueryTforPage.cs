using System;
using System.Linq.Expressions;

namespace Stupid.PageConfig
{
    /// <summary>
    /// 查询拓展，以便于分页处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryTforPage<T> where T : class
    {
        /// <summary>
        /// 查询拓展
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <param name="collectionPager">分页信息</param>
        public QueryTforPage(Expression<Func<T, bool>> expression, CollectionPager collectionPager)
        {
            this._CollectionPager = collectionPager;
            this.Expression = expression;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        public Expression<Func<T, bool>> Expression { get; set; }


        private Stupid.PageConfig.CollectionPager _CollectionPager { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public Stupid.PageConfig.CollectionPager CollectionPager
        {
            get
            {
                return _CollectionPager;
            }
            set
            {
                if (value.PageSize == 0)
                    value.PageSize = 15;
                _CollectionPager = value;
            }
        }
    }
}
