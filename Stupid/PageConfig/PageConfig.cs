using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.PageConfig
{
    /// <summary>
    /// 数据分页控制
    /// </summary>
    public class PageConfig
    {
        private CollectionPager _page { get; set; }

        private string _url;

        private string _mark;

        private int _pagecount;

        private int _pageother;

        /// <summary>
        /// 实例化PAGE
        /// 
        /// ex:
        ///  new PageConfig(new CollectionPager(10),"/Index?","Page");
        /// </summary>
        /// <param name="page">分页数据</param>
        /// <param name="url">链接地址</param>
        /// <param name="Mark">标记符</param>
        public PageConfig(CollectionPager page, string url, string Mark="Page")
        {
            _page = page ==null?new CollectionPager(10):page;
            _url = url;
            _mark = Mark;
            _pagecount = page.Count / page.PageSize;
            _pageother = page.Count % page.PageSize;
        }


        /// <summary>
        /// 获取动态分页，PageLength是只显示多少分页
        /// </summary>
        /// <param name="PageLength"></param>
        /// <param name="showall">是否显示全部</param>
        /// <param name="shownext">显示下一页</param>
        /// <returns></returns>
        public virtual string GetNavHTML(int PageLength = 6, bool shownext = false, bool showall = false)//关键字 virtual
        {
            try
            {
                var h_str = string.Empty;

                if (_pagecount == 1 && _page.Count % _page.PageSize == 0)//总分页小于2，并且第二分页没数据
                {
                    return string.Empty;
                }
                h_str = "<nav><ul class=\"pagination\">";//头部
                var z_str = "";
                var jishu = 0;
                var ts_str1 = "";//当总分页大于10时
                var ts_str2 = "";//当总分页大于10时

                if (_pageother > 0)
                {
                    _pagecount++;
                }
                if (_pagecount == 1)
                {
                    return string.Empty;
                }

                var upstr = "";
                var nextstr = "";




                if (_pagecount <= PageLength)//中部
                //if (PageCount >= 1)//中部
                {
                    //上一页
                    if (shownext)
                    {
                        if (_page.CurrentPage != 1)
                        {
                            upstr += "<li><a href=\"" + this._url + "" + _mark + "=" + (_page.CurrentPage - 1) + "\">上一页</a></li>";
                        }
                    }

                    for (int i = 0; i < _pagecount; i++)
                    {
                        jishu++;
                        if (jishu == _page.CurrentPage)
                        {
                            z_str += "<li class=\"active\"><span>" + jishu + " <span class=\"sr-only\">(current)</span></span></li>";
                        }
                        else
                        {
                            z_str += "<li><a href=\"" + _url + "" + _mark + "=" + jishu + "\">" + jishu + "</a></li>";
                        }
                    }


                    //下一页
                    if (shownext)
                    {
                        if (_page.CurrentPage != jishu)
                        {
                            nextstr += "<li><a href=\"" + _url + "" + _mark + "=" + (_page.CurrentPage + 1) + "\">下一页</a></li>";
                        }
                    }
                }
                else
                {
                    var count = _page.CurrentPage / PageLength;//记录是第几个大分页
                    var shengyu = _page.CurrentPage % PageLength;//记录余数
                    if (shengyu == 0 && count != 0)//当点击页数为整数时将大分页数减一
                    {
                        count--;
                    }

                    jishu = jishu + count * PageLength;


                    //上一页
                    if (shownext)
                    {
                        if (_page.CurrentPage != 1)
                        {
                            upstr += "<li><a href=\"" + _url + "" + _mark + "=" + (_page.CurrentPage - 1) + "\">上一页</a></li>";
                        }
                    }

                    var syys = _pagecount - (count * PageLength);//计算剩余页数
                    if (syys > PageLength)
                    {
                        for (int i = 0; i < PageLength; i++)
                        {
                            jishu++;
                            if (jishu == _page.CurrentPage)
                            {
                                z_str += "<li class=\"active\"><span>" + jishu + " <span class=\"sr-only\">(current)</span></span></li>";
                            }
                            else
                            {
                                z_str += "<li><a href=\"" + _url + "" + _mark + "=" + jishu + "\">" + jishu + "</a></li>";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < syys; i++)
                        {
                            jishu++;
                            if (jishu == _page.CurrentPage)
                            {
                                z_str += "<li class=\"active\"><span>" + jishu + " <span class=\"sr-only\">(current)</span></span></li>";
                            }
                            else
                            {
                                z_str += "<li><a href=\"" + _url + "" + _mark + "=" + jishu + "\">" + jishu + "</a></li>";
                            }
                        }
                    }

                    //下一页
                    if (shownext)
                    {
                        if (_page.CurrentPage != _pagecount)
                        {
                            nextstr += "<li><a href=\"" + _url + "" + _mark + "=" + (_page.CurrentPage + 1) + "\">下一页</a></li>";
                        }
                    }

                    if (!shownext)
                    {
                        if (count == 0)
                        {
                            ts_str1 = "<li class=\"disabled\"><a href=\"#\"><span aria-hidden=\"true\">«</span><span class=\"sr-only\">Previous</span></a></li>";
                        }
                        else
                        {
                            ts_str1 = "<li><a href=\"" + _url + "" + _mark + "=" + (count * PageLength) + "\"><span aria-hidden=\"true\">«</span><span class=\"sr-only\">Previous</span></a></li>";
                        }
                        if (syys > PageLength)
                        {
                            ts_str2 = "<li><a href=\"" + _url + "" + _mark + "=" + (jishu + 1) + "\"><span aria-hidden=\"true\">»</span><span class=\"sr-only\">Next</span></a></li>";
                        }
                        else
                        {
                            ts_str2 = "<li class=\"disabled\"><a href=\"#\"><span aria-hidden=\"true\">»</span><span class=\"sr-only\">Next</span></a></li>";
                        }
                    }
                }

                var e_li = "";

                //显示共几条记录
                if (showall)
                {
                    e_li = "<li><a>共<b>" + _page.Count + "</b>条记录</a></li>";
                }

                var e_str = "</ul></nav>";//尾部


                var result = h_str + ts_str1 + upstr + z_str + nextstr + ts_str2 + e_li + e_str;


                //_urlMark = "";//_url中的标记
                //PageCount = 0;//总分页数
                //PageSize = 0;//每页条数
                //Count = 0;//总条数
                //PageNo = 0;//当前分页
                return result;
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
