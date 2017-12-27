///********************************************************************
///参考 http://blog.csdn.net/honey199396/article/details/77849106
///********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.SomeConvert
{
    /// <summary>
    /// Unicode字符串转换帮助类
    /// </summary>
    public class UnicodeConvert
    {
        /// <summary>
        /// 字符串转Unicode码
        /// </summary>
        /// <returns>The to unicode.</returns>
        /// <param name="value">Value.</param>
        private string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串  
        /// </summary>
        /// <remarks>字符串特征为，通常会用‘u’然后紧接一组16进制的数字来表示这一个字符，一组16进制数字刚好是两个字符，和一个汉字长度相同。但是在Unicode编码转换成汉字的时候，采用的低字节序方式，例如：掉（\u6389），我们需要按照顺序”89”“63”来组合得到汉字“掉”。</remarks>
        /// <returns>The to string.</returns>
        /// <param name="unicode">Unicode.</param>
        private string UnicodeToString(string unicode)
        {
            string resultStr = "";
            string[] strList = unicode.Split('u');
            for (int i = 1; i < strList.Length; i++)
            {
                if (strList[i].Length > 4)
                {
                    strList[i] = strList[i].Substring(0, 4);
                }
                resultStr += (char)int.Parse(strList[i], System.Globalization.NumberStyles.HexNumber);
            }
            return resultStr;
        }
    }
}
