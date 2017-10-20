using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.PageConfig
{

    /// <summary>
    /// 页码帮助类
    /// </summary>
    [Serializable]
    [DataContract(Name = "CollectionPager")]
    public class CollectionPager
    {
        private int _pageSize;
        /// <summary>
        /// 分页大小
        /// </summary>
        [DataMember()]
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _currentPage;

        /// <summary>
        /// 当前页面的索引
        /// </summary>
        [DataMember()]
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        private int _count;

        /// <summary>
        /// 记录计数
        /// </summary>
        [DataMember()]
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                if (_pageSize > 0)
                    _pagers = _count / _pageSize;
            }
        }

        private int _pagers;
        /// <summary>
        /// 页面总数
        /// </summary>
        [DataMember()]
        public int Pagers
        {
            get
            {
                if (_count > 0 && _pageSize > 0)
                {
                    _pagers = _count / _pageSize;
                    if (_count % _pageSize > 0)
                    {
                        _pagers++;
                    }
                }

                return _pagers;
            }
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// 初始化分页信息
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="Count"></param>
        /// <param name="SortOrder"></param>
        public CollectionPager(int PageSize, int CurrentPage = 1, int Count = -1, SortOrder SortOrder = SortOrder.Ascending)
        {
            _pageSize = PageSize;
            _currentPage = CurrentPage;
            _count = Count;
            this.SortOrder = SortOrder;
        }
    }
}
