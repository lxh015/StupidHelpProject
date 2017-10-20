using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.PageConfig
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CollectionDataContract, Serializable]
    public class PageResult<T> : ICollection<T>
    {
        /// <summary>
        /// 分页信息对象
        /// </summary>
        [DataMember]
        public CollectionPager Pager { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> Data { get; set; }

        #region 重载PagedResult

        /// <summary>
        /// 空分页数据
        /// </summary>
        public PageResult()
        {
            Data = new List<T>();
        }

        /// <summary>
        /// 初始化分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <param name="data"></param>
        /// <param name="totalRecords"></param>
        public PageResult(int pageSize, int currentPage, List<T> data, int totalRecords = -1)
        {
            if (this.Pager == null)
                this.Pager = new CollectionPager(pageSize, currentPage);
            this.Pager.Count = totalRecords;
            this.Data = data;
        }
        #endregion

        /// <summary>
        /// 空分页
        /// </summary>
        public static readonly PageResult<T> Empty = new PageResult<T>(0, 0, null);


        #region 重写一些方法

        /// <summary>
        /// 指定的Object是否等于当前的Object
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == (object)null)
                return true;
            var other = obj as PageResult<T>;
            if (other == (object)null)
                return true;
            return this.Pager == other.Pager &&
                this.Data == other.Data;
        }

        /// <summary>
        /// 获取HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Pager.GetHashCode();
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(PageResult<T> a, PageResult<T> b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            return a.Equals(b);
        }

        /// <summary>
        /// 不等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(PageResult<T> a, PageResult<T> b)
        {
            return !(a == b);
        }

        #endregion

        #region IEnumerator方法

        /// <summary>
        /// 获取可遍历数组
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        #endregion

        #region ICollection<T>方法实现
        /// <summary>
        /// 将某项添加到 ICollection{T} 中。
        /// </summary>
        /// <param name="item">要添加到 ICollection{T} 的对象。</param>
        public void Add(T item)
        {
            Data.Add(item);
        }

        /// <summary>
        /// 从 ICollection{T} 中移除所有项。
        /// </summary>
        public void Clear()
        {
            Data.Clear();
        }

        /// <summary>
        /// 确定 ICollection{T} 是否包含特定值。
        /// </summary>
        /// <param name="item">要在 ICollection{T} 中定位的对象。</param>
        /// <returns>如果在 ICollection{T} 中找到 item，则为 true；否则为 false。</returns>
        public bool Contains(T item)
        {
            return Data.Contains(item);
        }

        /// <summary>
        /// 从特定的 Array 索引开始，将 ICollection{T} 的元素复制到一个 Array 中。
        /// </summary>
        /// <param name="array">作为从 ICollection{T} 复制的元素的目标的一维 Array。 Array 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此索引处开始进行复制。</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取 ICollection{T} 中包含的元素数。
        /// </summary>
        public int Count
        {
            get { return Data.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示 ICollection{T} 是否为只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从 ICollection{T} 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从 ICollection{T} 中移除的对象。</param>
        /// <returns>如果已从 ICollection{T} 中成功移除 item，则为 true；否则为 false。 如果在原始 ICollection{T} 中没有找到 item，该方法也会返回 false。 </returns>
        public bool Remove(T item)
        {
            return Data.Remove(item);
        }
        #endregion
    }

}
