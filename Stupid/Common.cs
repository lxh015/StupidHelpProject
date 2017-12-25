using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Stupid.Extensions;
using System.Drawing;
using System.Xml;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Stupid
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 图片处理
        /// </summary>
        public class ImageCommon
        {
            public static Image ChangeImage(Image image)
            {

                return null;
            }
        }

        private Common() { }

        /// <summary>
        /// 使同一时间获取不同的Random   unchecked关键字用于取消整型算术运算和转换的溢出检查。
        /// </summary>
        private static Random rd = new Random();
        /// <summary>
        /// 随机数函数
        /// </summary>
        /// <returns></returns>
        public static Random Ran()
        {
            try
            {
                //int iSeed = 10;
                Random ro = new Random(10);
                long tick = DateTime.Now.Ticks;
                Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                return ran;
            }
            catch
            {
                var ra = new Random();
                Random ran = new Random(Convert.ToInt32(ra));
                return ran;
            }
        }

        /// <summary>
        /// 检查类代码
        /// </summary>
        public class CheckCommon
        {
            #region 检查类代码

            /// <summary>
            /// 密码强度检测
            /// </summary>
            /// <param name="password"></param>
            /// <returns></returns>
            public static Int32 Password_Check(string password)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.Replace(password, "^(?:([a-z])|([A-Z])|([0-9])|(.)){6,}|(.)+$", "$1$2$3$4$5").Length;
                }
                catch
                {
                    return 0;
                }
            }

            /// <summary>
            /// 检查字符串手机号码是否正确
            /// </summary>
            /// <param name="mobile"></param>
            /// <param name="cl"></param>
            /// <returns></returns>
            public static bool Check_Moblie(string mobile, CMLevel cl = CMLevel.Simple)
            {
                try
                {
                    switch (cl)
                    {
                        default:
                            return System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^1[3|4|5|8|7][0-9]\d{8}$");
                        case CMLevel.Strict:
                            return System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^(0|86)?(13[0-9]|15[0-9]|17[0678]|18[0-9]|14[57])[0-9]{8}$");
                    }
                }
                catch
                {
                    return false;
                }
            }
            /// <summary>
            /// 检查手机级别
            /// </summary>
            public enum CMLevel
            {
                /// <summary>
                /// 简单
                /// </summary>
                Simple,
                /// <summary>
                /// 严格
                /// </summary>
                Strict,
            }


            /// <summary>
            /// 固定电话
            /// </summary>
            /// <param name="guhua"></param>
            /// <returns></returns>
            public static bool Check_Guhua(string guhua)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(guhua, @"^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$");
                }
                catch
                {

                    return false;
                }
            }

            /// <summary>
            /// 检查字符串是否为E-Mail
            /// </summary>
            /// <param name="email"></param>
            /// <returns></returns>
            public static bool Check_Email(string email)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 检查字符串是否为全数字
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool Check_Num(string str)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[0-9]*$");
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 检查字符串是否全为汉字
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool Check_Chinese(string str)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
                }
                catch
                {
                    return false;
                }
            }


            /// <summary>
            /// 判断字符串是否全为字母
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool Check_Englist(string str)
            {
                try
                {
                    return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Z]*$");
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 18位身份证检查
            /// </summary>
            /// <param name="CodeNo">身份证号码</param>
            /// <returns></returns>
            public static bool Check_ChineseID(string CodeNo)
            {
                if (CodeNo.Length != 18)
                    return false;



                string birth = CodeNo.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (!DateTime.TryParse(birth, out time))
                    return false;

                int[] xishu = new int[17] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                char[] first = CodeNo.Substring(0, 17).ToCharArray();
                int sum = 0;
                //防止出现非数字
                try
                {
                    for (int i = 0; i < xishu.Length; i++)
                    {
                        char item = first[i];
                        int ite = int.Parse(item.ToString());
                        sum += (xishu[i] * ite);
                    }
                }
                catch
                {
                    return false;
                }


                int other = sum % 11;
                if (other < 0 || other >= 11)
                    return false;

                string last = string.Empty;
                #region 验证最后一位
                switch (other)
                {
                    case 0:
                        last = "1";
                        break;
                    case 1:
                        last = "0";
                        break;
                    case 2:
                        last = "x";
                        break;
                    case 3:
                        last = "9";
                        break;
                    case 4:
                        last = "8";
                        break;
                    case 5:
                        last = "7";
                        break;
                    case 6:
                        last = "6";
                        break;
                    case 7:
                        last = "5";
                        break;
                    case 8:
                        last = "4";
                        break;
                    case 9:
                        last = "3";
                        break;
                    case 10:
                        last = "2";
                        break;
                    default:
                        return false;
                }
                #endregion

                string clast = CodeNo.Substring(17);

                if (last == clast)
                    return true;
                else if (last == clast.ToLower())
                    return true;

                return false;
            }

            #endregion
        }

        /// <summary>
        /// 加密类代码
        /// </summary>
        public class EncipherCommon
        {
            #region MD5

            /// <summary>
            /// 返回建立的MD5
            /// </summary>
            /// <param name="vname"></param>
            /// <returns></returns>
            public static string MD5(string vname)
            {
                return MakeMD5(vname);
            }

            /// <summary>
            /// 生成MD5
            /// </summary>
            /// <param name="vname"></param>
            /// <returns></returns>
            protected static string MakeMD5(string vname)
            {
                try
                {
                    //if (type == Md5Type.Thirty)
                    //{

                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    //获取密文字节数组 
                    byte[] byersult = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(vname));
                    //转换成字符串，32位 
                    var strresult = BitConverter.ToString(byersult);
                    //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉 
                    strresult = strresult.Replace("-", "");
                    return strresult;
                    //}
                    //else
                    //{
                    //    var str = "";
                    //    System.Security.Cryptography.MD5 md15 = System.Security.Cryptography.MD5.Create();
                    //    byte[] bye = md15.ComputeHash(System.Text.Encoding.UTF8.GetBytes(vname));
                    //    for (int i = 0; i < bye.Length; i++)
                    //    {
                    //        //16进制转换 
                    //        str = str + bye[i].ToString("X");
                    //    }
                    //    return str;
                    //}
                }
                catch
                {
                    return vname;
                }
            }
            //public enum Md5Type
            //{
            //    Sixteen,
            //    Thirty,
            //}

            #endregion

            #region DES

            private static string DesKey = "L5&2j_i-";

            #region 加密
            /// <summary>
            /// 进行DES加密
            /// </summary>
            /// <param name="mytxt"></param>
            /// <param name="DesKeys"></param>
            /// <returns></returns>
            public static string EnDes(string mytxt, string DesKeys)
            {
                if (!string.IsNullOrEmpty(DesKeys))
                {
                    return MakeDes(mytxt, DesKeys);
                }
                else
                {
                    return MakeDes(mytxt, DesKey);
                }
            }

            /// <summary>
            /// des加密
            /// </summary>
            /// <param name="mytxt"></param>
            /// <param name="DesKey"></param>
            /// <returns></returns>
            protected static string MakeDes(string mytxt, string DesKey)
            {
                try
                {
                    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                    {
                        byte[] inputbytearray;
                        inputbytearray = Encoding.UTF8.GetBytes(mytxt);


                        des.Key = ASCIIEncoding.ASCII.GetBytes(MakeMD5(DesKey).Substring(0, 8));
                        des.IV = ASCIIEncoding.ASCII.GetBytes(MakeMD5(DesKey).Substring(0, 8));

                        MemoryStream ms = new MemoryStream();
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputbytearray, 0, inputbytearray.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }

                        string str = Convert.ToBase64String(ms.ToArray());
                        ms.Close();
                        return str;
                    }
                }
                catch
                {
                    return mytxt;
                }

            }

            #endregion

            #region 解密

            /// <summary>
            /// 进行解密
            /// </summary>
            /// <param name="code"></param>
            /// <param name="DesKeys"></param>
            /// <returns></returns>
            public static string DeDes(string code, string DesKeys)
            {
                if (!string.IsNullOrEmpty(DesKeys))
                {
                    return SolveDes(code, DesKeys);
                }
                else
                {
                    return SolveDes(code, DesKey);
                }
            }

            /// <summary>
            /// des解密
            /// </summary>
            /// <param name="code"></param>
            /// <param name="DesKey"></param>
            /// <returns></returns>
            protected static string SolveDes(string code, string DesKey)
            {
                try
                {
                    using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                    {
                        byte[] inputbytearray;
                        inputbytearray = Convert.FromBase64String(code);
                        des.Key = ASCIIEncoding.ASCII.GetBytes(MakeMD5(DesKey).Substring(0, 8));
                        des.IV = ASCIIEncoding.ASCII.GetBytes(MakeMD5(DesKey).Substring(0, 8));
                        MemoryStream ms = new MemoryStream();
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputbytearray, 0, inputbytearray.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }
                        string str = Encoding.UTF8.GetString(ms.ToArray());
                        ms.Close();
                        return str;
                    }
                }
                catch
                {
                    return code;
                }
            }

            #endregion

            #endregion

            /// <summary>
            /// RSA加，解密类
            /// </summary>
            public class RSACommon
            {

                #region 私钥
                /// <summary>
                /// RSA私钥格式转换，java->.net
                /// </summary>
                /// <param name="privateKey">java生成的RSA私钥</param>
                /// <returns></returns>
                public static string RSAPrivateKeyJava2DotNet(string privateKey)
                {
                    RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

                    return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                        Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                        Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
                }

                /// <summary>
                /// RSA私钥格式转换，.net->java
                /// </summary>
                /// <param name="privateKey">.net生成的私钥</param>
                /// <returns></returns>
                public static string RSAPrivateKeyDotNet2Java(string privateKey)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(privateKey);
                    BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
                    BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
                    BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
                    BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
                    BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
                    BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
                    BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
                    BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));

                    RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);

                    PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
                    byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
                    return Convert.ToBase64String(serializedPrivateBytes);
                }
                #endregion

                #region 公钥
                /// <summary>
                /// RSA公钥格式转换，java->.net
                /// </summary>
                /// <param name="publicKey">java生成的公钥</param>
                /// <returns></returns>
                public static string RSAPublicKeyJava2DotNet(string publicKey)
                {
                    RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
                    return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                        Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                        Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
                }

                /// <summary>
                /// RSA公钥格式转换，.net->java
                /// </summary>
                /// <param name="publicKey">.net生成的公钥</param>
                /// <returns></returns>
                public static string RSAPublicKeyDotNet2Java(string publicKey)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(publicKey);
                    BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
                    BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
                    RsaKeyParameters pub = new RsaKeyParameters(false, m, p);

                    SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
                    byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
                    return Convert.ToBase64String(serializedPublicBytes);
                }
                #endregion
            }
        }

        /// <summary>
        /// 将时间转化为长数据时间
        /// </summary>
        public class ConvertTimeCommon
        {
            #region 时间转换代码



            /// <summary>
            /// 将DateTime时间格式转换为Unix时间戳格式,毫秒
            /// </summary>
            /// <param name="time">时间</param>
            /// <returns>13位时间戳！</returns>
            public static long GetLongtime(System.DateTime time)
            {
                try
                {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    //double intResult = 0;
                    //intResult = (time - startTime).TotalMilliseconds;
                    //return intResult;
                    long t = (time.Ticks - startTime.Ticks) / 10000;     //除10000调整为13位
                    return t;
                }
                catch
                {
                    return new long();
                }
            }

            /// <summary>
            /// 将Long类型转换为DateTime类型
            /// </summary>
            /// <param name="time">13位时间戳！</param>
            /// <returns></returns>
            public static DateTime GetDatetime(long time)
            {
                try
                {
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    long lTime = long.Parse(time + "0000");
                    TimeSpan toNow = new TimeSpan(lTime);
                    return dtStart.Add(toNow);
                    //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    //long lTime = long.Parse((time - 8 * 60 * 60 * 1000) + "0000");
                    //TimeSpan toNow = new TimeSpan(lTime);
                    //DateTime dtResult = dtStart.Add(toNow);
                    //return dtResult;
                }
                catch
                {
                    return new DateTime();
                }
            }

            /// <summary>
            /// 将10位时间戳转换为正常时间
            /// </summary>
            /// <param name="curSeconds">十位时间戳</param>
            /// <returns>时间</returns>
            public static DateTime GetDateTimeForTen(long curSeconds)
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                return dtStart.AddSeconds(curSeconds);
            }

            #endregion
        }

        /// <summary>
        /// 验证码类代码
        /// </summary>
        public class CodeCommon
        {
            /// <summary>
            /// 其中All包括数字和字母，不包括汉字
            /// </summary>
            public enum CodeType
            {
                /// <summary>
                /// 数字
                /// </summary>
                Num = 1,

                /// <summary>
                /// 英文字母
                /// </summary>
                English = 2,

                /// <summary>
                /// 汉字
                /// </summary>
                Chinese = 3,

                /// <summary>
                /// 字母和数字（不包括汉字）
                /// </summary>
                All = 4,
            }
            /// <summary>
            /// 制作验证码，非图片（未完善）
            /// </summary>
            /// <returns></returns>
            public static string CreateCode(CodeType type, int length = 4)
            {
                var ran = rd;
                var arr = new ArrayList();
                var code = string.Empty;

                var time = DateTime.Now;
                var miao = time.Millisecond;
                var zongchang = 0.0;

                if (miao < 10 && miao > 0)
                {
                    zongchang = (miao / 10) * ran.NextDouble() * 13;
                }
                else if (miao == 0)
                {
                    miao = ran.Next(1, 9);
                    zongchang = (miao / 3) * 13;
                }
                else if (miao > 100)
                {
                    zongchang = (miao / 100) * 13;
                }
                else
                {
                    zongchang = (miao / 10) * 13;
                }

                var r_zongchang = Convert.ToInt32(zongchang);
                switch (type)
                {
                    case CodeType.Num:

                        for (int i = 0; i < length; i++)
                        {
                            var new_r3 = rd;
                            var y = new_r3.Next(1, 10);
                            arr.Add(y);
                        }
                        break;
                    case CodeType.English:
                        for (int i = 0; i < length; i++)
                        {
                            var new_r = rd;
                            var new_r1 = rd;
                            var new_r2 = rd;
                            var rr = new_r.Next(0, r_zongchang);
                            if (rr % 2 == 0)
                            {
                                var y = (char)new_r1.Next(65, 90);
                                arr.Add(y);
                            }
                            else
                            {
                                var y = (char)new_r2.Next(97, 122);
                                arr.Add(y);
                            }
                        }
                        break;
                    case CodeType.Chinese:

                        string checkCode = String.Empty;

                        Encoding gb = Encoding.GetEncoding("gb2312");
                        //调用函数产生4个随机中文汉字编码 柯乐义
                        object[] bytes = CreateRegionCode(length);
                        //根据汉字编码的字节数组解码出中文汉字 
                        for (int i = 0; i < length; i++)
                        {
                            string str = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));
                            arr.Add(str);
                        }
                        break;
                    case CodeType.All:
                        for (int i = 0; i < length; i++)
                        {
                            var new_r = rd;
                            var new_r1 = rd;
                            var new_r2 = rd;
                            var new_r3 = rd;
                            var r = ran.Next(0, r_zongchang);
                            if (r % 2 == 0)
                            {
                                var rr = new_r.Next(0, r_zongchang);
                                if (rr % 2 == 0)
                                {
                                    var y = (char)new_r1.Next(65, 90);
                                    arr.Add(y);
                                }
                                else
                                {
                                    var y = (char)new_r2.Next(97, 122);
                                    arr.Add(y);
                                }
                            }
                            else
                            {
                                var y = new_r3.Next(1, 10);
                                arr.Add(y);
                            }
                        }
                        break;
                    default:
                        return string.Empty;
                }

                for (int i = 0; i < length; i++)
                {
                    code += arr[i].ToString();
                }

                //将code加入到Session中
                //HttpContext.Current.Session.Add("LCode", code);
                return code;
            }


            /// <summary>
            /// 生成汉字
            /// </summary>
            /// <param name="strlength"></param>
            /// <returns></returns>
            private static object[] CreateRegionCode(int strlength)
            {
                //定义一个字符串数组储存汉字编码的组成元素 
                string[] rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
                var rnd = rd;
                //定义一个object数组用来 
                object[] bytes = new object[strlength];

                /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bject数组中 
                每个汉字有四个区位码组成 
                区位码第1位和区位码第2位作为字节数组第一个元素 
                区位码第3位和区位码第4位作为字节数组第二个元素 
                */
                for (int i = 0; i < strlength; i++)
                {
                    //区位码第1位 
                    int r1 = rnd.Next(11, 14);
                    string str_r1 = rBase[r1].Trim();

                    //区位码第2位 
                    rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的

                    //种子避免产生重复值 
                    int r2;
                    if (r1 == 13)
                    {
                        r2 = rnd.Next(0, 7);
                    }
                    else
                    {
                        r2 = rnd.Next(0, 16);
                    }
                    string str_r2 = rBase[r2].Trim();

                    //区位码第3位 
                    rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                    int r3 = rnd.Next(10, 16);
                    string str_r3 = rBase[r3].Trim();

                    //区位码第4位 
                    rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                    int r4;
                    if (r3 == 10)
                    {
                        r4 = rnd.Next(1, 16);
                    }
                    else if (r3 == 15)
                    {
                        r4 = rnd.Next(0, 15);
                    }
                    else
                    {
                        r4 = rnd.Next(0, 16);
                    }
                    string str_r4 = rBase[r4].Trim();

                    //定义两个字节变量存储产生的随机汉字区位码
                    byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                    byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                    //将两个字节变量存储在字节数组中 
                    byte[] str_r = new byte[] { byte1, byte2 };

                    //将产生的一个汉字的字节数组放入object数组中 
                    bytes.SetValue(str_r, i);
                }
                return bytes;
            }

        }

        /// <summary>
        /// 应用类代码
        /// </summary>
        public class ApplyCommon
        {
            /// <summary>
            /// 获取屏幕工作宽度
            /// </summary>
            /// <returns></returns>
            public static int Screen_Width()
            {
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
                return screen.WorkingArea.Width;
            }

            /// <summary>
            /// 获取屏幕工作高度
            /// </summary>
            /// <returns></returns>
            public static int Screen_Height()
            {
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
                return screen.WorkingArea.Height;
            }

            /// <summary>
            /// 获取当前生成目录
            /// </summary>
            /// <remarks>生成目录一般指\bin\Debug</remarks>
            /// <returns></returns>
            public static string GetBllPath()
            {
                return System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }

            /// <summary>
            /// 截取字符串|根据字符串中的字符个数,英文一个占位，中文两个占位
            /// </summary>
            /// <param name="vname"></param>
            /// <param name="length"></param>
            /// <param name="endstr"></param>
            /// <returns></returns>
            public static string GetString(string vname, int length, string endstr = "")
            {
                try
                {
                    var str = "";
                    var vname_length = vname.Length;
                    var str_length = 0;
                    var cutstr = "";


                    var shijileng = 0;
                    byte[] bytes;
                    Encoding encoding = Encoding.GetEncoding("gb2312");
                    var cut_length = 0;


                    Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
                    string strOutput = regex.Replace(vname, "");
                    cutstr = strOutput;

                    for (int i = 0; i < vname_length; i++)//判断是中文还是英文
                    {
                        bytes = encoding.GetBytes(vname.Substring(i, 1));
                        if (bytes.Length == 2)
                        {
                            shijileng += 2;
                        }
                        else
                        {
                            shijileng++;
                        }

                        if (shijileng <= length)
                        {
                            cut_length++;
                        }


                        if (shijileng > length)
                        {
                            str = cutstr.Substring(0, cut_length);
                        }
                    }

                    if (shijileng <= length)
                    {
                        str = vname;
                    }

                    //try
                    //{
                    //    str = vname.Substring(0, length);
                    //}
                    //catch
                    //{
                    //    str = vname;
                    //}

                    if (endstr == "")
                    {
                        return str;
                    }
                    else
                    {
                        if (vname_length == str_length || vname_length <= length)
                        {
                            return str;
                        }
                        else
                        {
                            return str + endstr;
                        }
                    }
                }
                catch
                {
                    return vname;
                }
            }

            /// <summary>
            /// 判断乱码(Utf-8编码是否乱码)
            /// </summary>
            /// <param name="txt">字符串</param>
            /// <returns>true:乱码</returns>
            public static bool UTF8IsNoMath(string txt)
            {
                var bytes = Encoding.UTF8.GetBytes(txt);
                //239 191 189
                for (var i = 0; i < bytes.Length; i++)
                {
                    if (i < bytes.Length - 3)
                        if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                        {
                            return true;
                        }
                }
                return false;
            }

        }

        /// <summary>
        ///  网站应用类代码
        /// </summary>
        public class WebApplyCommon
        {
            /// <summary>
            /// 返回上一页
            /// </summary>
            /// <returns></returns>
            public static string GetUrlReferrer()
            {
                try
                {
                    var a = HttpContext.Current.Request.UrlReferrer.ToString();
                    return a;
                }
                catch
                {
                    return "/";
                }
            }

            /// <summary>
            /// 获取网站中的web中的设置
            /// </summary>
            /// <param name="vname"></param>
            /// <returns></returns>
            public static string GetAppSettings(string vname)
            {
                try
                {
                    var r = System.Configuration.ConfigurationManager.AppSettings[vname];
                    return r;
                }
                catch
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 使用本地方法获取IP
            /// </summary>
            /// <returns></returns>
            public static string GetIPInfo()
            {
                try
                {
                    System.Net.IPAddress[] addrs;
                    string ipAddr = string.Empty;
                    addrs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()); // 获得本机局域网IP地址 

                    if (addrs.Length > 1)
                    {
                        foreach (System.Net.IPAddress addr in addrs)
                        {
                            //if (!addr.IsIPv6LinkLocal)
                            //{
                            //    ipAddr = addr.ToString();
                            //    break;
                            //}
                            if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipAddr = addr.ToString();
                                break;
                            }
                        }
                        return ipAddr;
                    }
                    return addrs[1].ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// 通过查询外网获取IP
            /// </summary>
            /// <returns></returns>
            public static string GetOutIP()
            {
                // string strUrl = "http://www.ip138.com/ip2city.asp"; //获得IP的网址了 
                string strUrl = "http://1212.ip138.com/ic.asp"; //获得IP的网址了 
                //Uri uri = new Uri(strUrl);
                //System.Net.WebRequest wr = System.Net.WebRequest.Create(uri);
                //System.IO.Stream s = wr.GetResponse().GetResponseStream();
                //System.IO.StreamReader sr = new System.IO.StreamReader(s, Encoding.Default);
                string all = HttpClientHelper.HttpGet(strUrl, ""); //读取网站的数据           
                                                                   // int i = all.IndexOf("[") + 1;
                int i = all.LastIndexOf("[") + 1;
                string tempip = all.Substring(i, 15);
                string ip = tempip.Replace("]", "").Replace(" ", "");//找出i
                                                                     //也可用
                                                                     //new Regex(@"ClientIP: \[([\d.]+?)\]").Match(new System.Net.WebClient().DownloadString("http://www.skyiv.com/info/")).Groups[1].Value;
                return ip;
            }

            /// <summary>
            /// 获取访问端IP地址
            /// </summary>
            /// <returns></returns>
            public static string GetOutUserIP()
            {
                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                try
                {
                    var iplist = ip.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (iplist.Count() == 0)
                        return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    else
                    {
                        return iplist[0];
                    }
                }
                catch
                {
                    return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }

            /// <summary>
            /// 获取访问端浏览器名称
            /// </summary>
            /// <returns></returns>
            public static string GetBrowser()
            {
                return HttpContext.Current.Request.Browser.ToString();
            }
        }

        /// <summary>
        /// 自定义应用类代码
        /// </summary>
        public class MyApplyCommon
        {
            /// <summary>
            /// 根据时间创建一个文件名
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public static string Create_name(CNameType type = CNameType.Long)
            {
                var date = DateTime.Now;
                var name = "";
                switch (type)
                {
                    case CNameType.Short:
                        name = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + rd.Next(0, 9).ToString();
                        break;
                    default:
                        name = date.Year.ToString() + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + date.Millisecond.ToString() + rd.Next(0, 9).ToString();
                        break;
                }
                return name;
            }
            /// <summary>
            /// 名称类别
            /// </summary>
            public enum CNameType
            {
                /// <summary>
                /// 长名称（包含毫秒）
                /// </summary>
                Long,
                /// <summary>
                /// 短名称（不包含毫秒）
                /// </summary>
                Short,
            }

            /// <summary>
            ///  将手机号码中，中间的数字换成***
            /// </summary>
            /// <param name="phone"></param>
            /// <param name="ptype"></param>
            /// <returns></returns>
            public static string Change_phone(string phone, PhoneType ptype)
            {
                try
                {
                    if (ptype == PhoneType.moblie)
                    {
                        var np = phone.Remove(3);
                        var ndp = phone.Remove(0, phone.Length - 3);
                        var rp = np + "****" + ndp;
                        return rp;
                    }
                    else if (ptype == PhoneType.fixed_phone)
                    {
                        var ps = phone.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        var qh = ps[0];
                        var p = ps[1];
                        var np = p.Remove(3);
                        var ndp = p.Remove(p.Length - 2);
                        var rp = qh + "-" + np + "***" + ndp;
                        return rp;
                    }
                    else
                    {
                        return phone;
                    }
                }
                catch
                {
                    return phone;
                }
            }
            /// <summary>
            /// 选择号码类型
            /// </summary>
            public enum PhoneType
            {
                /// <summary>
                /// 手机
                /// </summary>
                moblie,

                /// <summary>
                /// 固定电话
                /// </summary>
                fixed_phone,
            }

            /// <summary>
            /// 将数据加入到缓存中,其中continues为保存秒数
            /// </summary>
            /// <param name="key_name"></param>
            /// <param name="value"></param>
            /// <param name="continues"></param>
            /// <returns></returns>
            public bool SaveCache(string key_name, object value, Double continues)
            {
                try
                {
                    HttpContext.Current.Cache.Insert(key_name, value, null, DateTime.Now.AddSeconds(continues), System.Web.Caching.Cache.NoSlidingExpiration);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 第一个值为前缀，如果出错将返回2015-9-7格式
            /// </summary>
            /// <param name="v_prefix"></param>
            /// <param name="time"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static string GetWeek(string v_prefix, DateTime time, WeekStrType type = WeekStrType.Chinese)
            {
                try
                {
                    var str_week = "";
                    var week = time.DayOfWeek;
                    if (type == WeekStrType.Chinese)
                    {
                        switch (week)
                        {
                            case DayOfWeek.Friday:
                                str_week = v_prefix + "五";
                                break;
                            case DayOfWeek.Monday:
                                str_week = v_prefix + "一";
                                break;
                            case DayOfWeek.Saturday:
                                str_week = v_prefix + "六";
                                break;
                            case DayOfWeek.Sunday:
                                str_week = v_prefix + "日";
                                break;
                            case DayOfWeek.Thursday:
                                str_week = v_prefix + "四";
                                break;
                            case DayOfWeek.Tuesday:
                                str_week = v_prefix + "二";
                                break;
                            case DayOfWeek.Wednesday:
                                str_week = v_prefix + "三";
                                break;
                        }
                    }
                    else
                    {
                        switch (week)
                        {
                            case DayOfWeek.Friday:
                                str_week = v_prefix + "5";
                                break;
                            case DayOfWeek.Monday:
                                str_week = v_prefix + "1";
                                break;
                            case DayOfWeek.Saturday:
                                str_week = v_prefix + "6";
                                break;
                            case DayOfWeek.Sunday:
                                str_week = v_prefix + "7";
                                break;
                            case DayOfWeek.Thursday:
                                str_week = v_prefix + "4";
                                break;
                            case DayOfWeek.Tuesday:
                                str_week = v_prefix + "2";
                                break;
                            case DayOfWeek.Wednesday:
                                str_week = v_prefix + "3";
                                break;
                        }
                    }
                    return str_week;
                }
                catch
                {
                    return time.ToString("yyyy-MM-dd");
                }
            }
            /// <summary>
            /// 返回是汉字，还是数字
            /// </summary>
            public enum WeekStrType
            {
                /// <summary>
                /// 星期表示，如：一
                /// </summary>
                Chinese,
                /// <summary>
                /// 星期表示，如：1
                /// </summary>
                Num,
            }

            /// <summary>
            /// 获取昨天开始日期
            /// </summary>
            /// <returns></returns>
            public static DateTime GetYesterdayStar()
            {
                var yesterday = DateTime.Now.AddDays(-1).ToShortDateString();
                return Convert.ToDateTime(yesterday);
            }

            /// <summary>
            /// 获取今天开始日期
            /// </summary>
            /// <returns></returns>
            public static DateTime GetTodayStar()
            {
                var today = DateTime.Now.ToShortDateString();
                return Convert.ToDateTime(today);
            }
        }

        /// <summary>
        /// 检查文件格式类代码
        /// </summary>
        public class CheckFileTypeCommon
        {
            #region 检查文件格式代码



            /// <summary>
            /// 检查文件是否为图片，后缀名
            /// </summary>
            /// <param name="image_name"></param>
            /// <returns></returns>
            public static bool IsImage(string image_name)
            {
                try
                {
                    var lowname = image_name.ToLower();
                    var result = false;
                    if (image_name.Contains("bmp") || image_name.Contains("pcx") || image_name.Contains("tiff") || image_name.Contains("gif") || image_name.Contains("jpg"))
                    {
                        result = true;
                    }
                    else if (image_name.Contains("jpeg") || image_name.Contains("tga") || image_name.Contains("exif") || image_name.Contains("fpx") || image_name.Contains("svg"))
                    {
                        result = true;
                    }
                    else if (image_name.Contains("psd") || image_name.Contains("cdr") || image_name.Contains("pcd") || image_name.Contains("dxf") || image_name.Contains("ufo"))
                    {
                        result = true;
                    }
                    else if (image_name.Contains("eps") || image_name.Contains("ai") || image_name.Contains("png") || image_name.Contains("hdri") || image_name.Contains("raw"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    return result;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 检查文件是否为视频，后缀名
            /// </summary>
            /// <param name="video_name"></param>
            /// <returns></returns>
            public static bool IsVideo(string video_name)
            {
                try
                {
                    var lowname = video_name.ToLower();
                    var result = false;
                    if (video_name.Contains("mpeg") || video_name.Contains("mpg") || video_name.Contains("dat") || video_name.Contains("avi") || video_name.Contains("ra"))
                    {
                        result = true;
                    }
                    else if (video_name.Contains("wmv") || video_name.Contains("asf") || video_name.Contains("mov") || video_name.Contains("ram") || video_name.Contains("rm"))
                    {
                        result = true;
                    }
                    else if (video_name.Contains("navi") || video_name.Contains("divx") || video_name.Contains("rmvb") || video_name.Contains("flv") || video_name.Contains("f4v"))
                    {
                        result = true;
                    }
                    else if (video_name.Contains("mp4") || video_name.Contains("3gp") || video_name.Contains("amv"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    return result;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 检查文件是否为音频，后缀名
            /// </summary>
            /// <param name="audio_name"></param>
            /// <returns></returns>
            public static bool IsAudio(string audio_name)
            {
                try
                {
                    var lowname = audio_name.ToLower();
                    var result = false;
                    if (audio_name.Contains("cd") || audio_name.Contains("wave") || audio_name.Contains("aiff") || audio_name.Contains("au") || audio_name.Contains("mpeg"))
                    {
                        result = true;
                    }
                    else if (audio_name.Contains("realaudio") || audio_name.Contains("wma") || audio_name.Contains("midi") || audio_name.Contains("mpeg-4") || audio_name.Contains("mp3"))
                    {
                        result = true;
                    }
                    else if (audio_name.Contains("vqf") || audio_name.Contains("oggvorbis") || audio_name.Contains("amr"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    return result;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 检查文件是否为FLASH，后缀名
            /// </summary>
            /// <param name="flash_name"></param>
            /// <returns></returns>
            public static bool IsFlash(string flash_name)
            {
                try
                {
                    var lowname = flash_name.ToLower();
                    var result = false;
                    if (flash_name.Contains("fla") || flash_name.Contains("swf"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    return result;
                }
                catch
                {
                    return false;
                }
            }

            #endregion
        }

        /// <summary>
        /// 自定义密钥
        /// </summary>
        public class MyEncipherCommon
        {
            #region 密钥建立

            /// <summary>
            /// 建立密钥
            /// </summary>
            /// <returns></returns>
            public static string Create_Lock()
            {
                var s_lock = new string[3];
                var r = Ran().Next(1, 3);//记录是否为单数
                var r_lock = "";

                var r1 = 0;//建立第一位密码索引
                var s1 = new char();
                var j1 = 0;
                if (r % 2 == 0)
                {
                    r1 = Ran().Next(97, 122);
                    s1 = (char)r1;
                    s_lock[0] = s1.ToString();
                    j1 = (r1 - 97) + 1;

                }
                else
                {
                    r1 = Ran().Next(0, 9);
                    s_lock[0] = r1.ToString();
                    j1 = r1 + 1;
                }

                s_lock[0] += Create_Code(j1 - 1);
                s_lock[0] += "L";//第一位密码索引结束


                var r2 = 0;//建立第二位密码索引
                var s2 = new char();
                var j2 = 0;
                if (r % 2 == 0)
                {
                    r2 = Ran().Next(0, 9);
                    s_lock[1] = r2.ToString();
                    j2 = r2 + 1;
                }
                else
                {
                    r2 = Ran().Next(97, 122);
                    s2 = (char)r2;
                    s_lock[1] = s2.ToString();
                    j2 = (r2 - 97) + 1;

                }
                s_lock[1] += Create_Code(j2 - 1);


                var r3 = 0;//建立第二位密码索引
                var s3 = new char();
                var j3 = 0;
                if (r % 2 == 0)
                {
                    r3 = Ran().Next(0, 9);
                    s_lock[2] = r3.ToString();
                    j3 = r3 + 1;
                }
                else
                {
                    r3 = Ran().Next(97, 122);
                    s3 = (char)r3;
                    s_lock[2] = s3.ToString();
                    j3 = (r3 - 97) + 1;

                }

                s_lock[2] += Create_Code(j3 - 1);


                r_lock = s_lock[0].ToString() + s_lock[1].ToString() + s_lock[2].ToString();

                var r_lock_length = r_lock.Length;
                if (r_lock_length <= 32)
                {
                    var shao = 32 - r_lock_length;
                    r_lock += Create_Code(shao);
                }
                else
                {
                    r_lock = r_lock.Remove(32);
                }


                var date = DateTime.Now;
                r_lock += Create_Date();

                return r_lock;
            }
            private static string Create_Date()
            {
                var date = DateTime.Now;
                string str = "";
                str = date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() + "|" + date.Hour.ToString() + "-" + date.Minute.ToString() + "-" + date.Second.ToString();
                return str;
            }

            private static bool Check_M_Date(string mdate)
            {
                try
                {
                    var spm = mdate.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);//将数据分隔
                    if (spm.Length != 2)//分隔成2个部分日期和时间
                    {
                        return false;
                    }
                    var sf = spm[0].ToString();//日期
                    var ss = spm[1].ToString();//时间

                    var spsf = sf.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    var spss = ss.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                    if (spsf.Length != 3 || spss.Length != 3)//判断数据分隔后是否符合
                    {
                        return false;
                    }

                    var yi = 1;
                    foreach (var item in spsf)
                    {
                        var math = CheckCommon.Check_Num(item);
                        if (math)
                        {
                            if (yi == 1)
                            {
                                if (item.Length != 4)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (item.Length < 1 || item.Length > 3)
                                {
                                    return false;
                                }
                            }
                            yi++;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    foreach (var item in spss)
                    {
                        var math = CheckCommon.Check_Num(item);
                        if (math)
                        {
                            if (item.Length < 1 || item.Length > 3)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 为密钥建立乱码
            /// </summary>
            /// <returns></returns>
            private static string Create_Code(int length = 4)
            {
                var ran = rd;
                var arr = new ArrayList();
                var code = string.Empty;
                var time = DateTime.Now;
                var miao = time.Millisecond;
                var zongchang = 0.0;

                if (miao < 10 && miao > 0)
                {
                    zongchang = (miao / 10) * ran.NextDouble() * 13;
                }
                else if (miao == 0)
                {
                    miao = ran.Next(1, 9);
                    zongchang = (miao / 3) * 13;
                }
                else if (miao > 100)
                {
                    zongchang = (miao / 100) * 13;
                }
                else
                {
                    zongchang = (miao / 10) * 13;
                }

                var r_zongchang = Convert.ToInt32(zongchang);

                for (int i = 0; i < length; i++)
                {
                    var new_r = rd;
                    var new_r1 = rd;
                    var new_r2 = rd;
                    var new_r3 = rd;
                    var r = ran.Next(0, r_zongchang);
                    if (r % 2 == 0)
                    {
                        var rr = new_r.Next(0, r_zongchang);
                        if (rr % 2 == 0)
                        {
                            var y = (char)new_r1.Next(65, 90);
                            arr.Add(y);
                        }
                        else
                        {
                            var y = (char)new_r2.Next(97, 122);
                            arr.Add(y);
                        }
                    }
                    else
                    {
                        var y = new_r3.Next(1, 10);
                        arr.Add(y);
                    }
                }

                for (int i = 0; i < length; i++)
                {
                    code += arr[i].ToString();
                }

                return code;
            }

            /// <summary>
            /// 解码
            /// </summary>
            /// <param name="Lock"></param>
            /// <returns></returns>
            protected static bool Is_Lock(string Lock)
            {
                if (string.IsNullOrEmpty(Lock))
                {
                    return false;
                }
                var mdate = Lock.Remove(0, 32);

                if (!Check_M_Date(mdate))
                {
                    return false;
                }

                if (Lock.Length < 45 || Lock.Length > 51)
                {
                    return false;
                }

                var f = Lock.Remove(1);//取首位字符
                var c_n = CheckCommon.Check_Num(f);

                var f_l = 0;//1为数字，2为字母
                if (c_n == false)
                {
                    var c_e = CheckCommon.Check_Englist(f);
                    if (c_e == false)
                    {
                        return false;
                    }
                    else
                    {
                        f_l = 2;
                    }
                }
                else
                {
                    f_l = 1;
                }

                var ismath = false;



                if (f_l == 0)
                {
                    return false;
                }
                else if (f_l == 1)
                {
                    var num = 1 + Convert.ToInt32(f);
                    var new_str = Lock.Remove(num + 1);
                    if (new_str.Last().ToString() == "L")
                    {
                        ismath = true;
                    }
                }
                else if (f_l == 2)
                {
                    var num = Get_English_Num(f);
                    if (num == 0)
                    {
                        ismath = false;
                    }
                    else
                    {
                        var new_str = Lock.Remove(num + 1);
                        if (new_str.Last().ToString() == "L")
                        {
                            ismath = true;
                        }
                    }
                }
                var rmlock = Lock.Remove(0, 32);
                var rrmlock = rmlock.Remove(rmlock.Length - 1);

                return ismath;
            }

            /// <summary>
            /// 获取单个英文字符在表中的位置
            /// </summary>
            /// <returns></returns>
            private static int Get_English_Num(string english)
            {
                var arr = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                var low_str = english.ToLower();
                var r_i = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (low_str == arr[i].ToString().ToLower())
                    {
                        r_i = i;
                    }
                }
                return r_i + 1;
            }

            #endregion
        }

        /// <summary>
        /// 数据压缩
        /// </summary>
        public class DataCompression
        {
            /// <summary>
            /// 压缩
            /// </summary>
            /// <param name="input">要压缩的信息,若是Class数据，可以先进行Json序列化</param>
            /// <returns>返回base64编码字符串</returns>
            public static string Compress(object input)
            {
                string writestr = string.Empty;
                if (input.GetType().IsClass)
                {
                    var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                    writestr = json.Serialize(input);
                }
                else
                    writestr = Convert.ToString(input);


                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                var bw = new BinaryWriter(ms, Encoding.Unicode);
                bw.Write(writestr);

                var dest = new MemoryStream();
                System.IO.Compression.GZipStream zip = new System.IO.Compression.GZipStream(dest, System.IO.Compression.CompressionMode.Compress, true);
                byte[] buffer = new byte[1024];
                int length;
                ms.Seek(0, SeekOrigin.Begin);

                while ((length = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    zip.Write(buffer, 0, length);
                }

                zip.Close();
                bw.Close();
                ms.Close();


                var cb = dest.ToArray();
                string info = Convert.ToBase64String(cb);


                return info;
            }


            /// <summary>
            /// 解压
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="data">解压的数据</param>
            /// <returns></returns>
            public static T Decompress<T>(string data)
            {
                //var bytes = System.Text.Encoding.ASCII.GetBytes(data);
                var bytes = Convert.FromBase64String(data);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var dest = new MemoryStream();

                System.IO.Compression.GZipStream zip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress, true);

                byte[] buffer = new byte[1024];
                int length;
                while ((length = zip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dest.Write(buffer, 0, length);
                }
                zip.Close();
                dest.Seek(0, SeekOrigin.Begin);

                var br = new BinaryReader(dest, Encoding.Unicode);

                string info = br.ReadString();
                var type = typeof(T);
                if (type.IsClass)
                {
                    var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                    return json.Deserialize<T>(info);
                }
                else
                    return Extensions.ConvertEntensions.ConvertType<T>(info);
            }
        }

        /// <summary>
        /// 转换类
        /// </summary>
        public class ConvertHander
        {
            /// <summary>
            /// 深拷贝。以防止地址引用。
            ///  
            /// 备注：
            /// 未被标记为可序列化,解决方案：类继承MarshalByRefObject，同时加上<code>Serializable</code>特性
            /// </summary>
            /// <exception cref="Exception"></exception>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <returns>出现异常时返回一个默认空类。</returns>
            public static T DeepCopy<T>(T obj)
            {
                try
                {
                    object retval;
                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        //序列化成流
                        bf.Serialize(ms, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        //反序列化成对象
                        retval = bf.Deserialize(ms);
                        ms.Close();
                    }
                    return (T)retval;
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }
}
