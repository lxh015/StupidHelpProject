using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Stupid
{
    /// <summary>
    /// 获取验证码 For Web
    /// </summary>
    public class MakeVCodeForWeb
    {
        /// <summary>
        /// 记录code
        /// </summary>
        public string vcode;

        /// <summary>
        /// 选择code样式
        /// </summary>
        public enum CodeType
        {
            /// <summary>
            /// 数字
            /// </summary>
            Num,
            /// <summary>
            /// 英文
            /// </summary>
            English,
            /// <summary>
            /// 中文
            /// </summary>
            Chinese,
            /// <summary>
            /// 数字加英文
            /// </summary>
            All,
        }


        /// <summary>
        /// 随机数
        /// </summary>
        private static Random rd = new Random();

        #region 旧版
        //public void Page_Load(object sender, EventArgs e)
        //{
        //    string chkCode = string.Empty;

        //    //颜色列表，用于验证码，噪线，噪点
        //    Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue };

        //    //字体列表，用于验证码
        //    string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };

        //    //验证码的字符集
        //    string[] character = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        //    var rnd = rd;

        //    //生成验证码字符串
        //    for (int i = 0; i < 4; i++)
        //    {
        //        chkCode += character[rnd.Next(character.Length)];
        //    }

        //    //保存验证码
        //    vcode = chkCode;

        //    //保存验证码Cookie
        //    HttpCookie anycookie = new HttpCookie("validateCookie");
        //    anycookie.Values.Add("ChkCookie", chkCode);
        //    HttpContext.Current.Response.Cookies["validateCookie"].Values["ChkCookie"] = chkCode;

        //    //创建bitmap
        //    Bitmap bmp = new Bitmap(150, 30);
        //    Graphics g = Graphics.FromImage(bmp);
        //    g.Clear(Color.White);

        //    //画噪线
        //    for (int i = 0; i < 5; i++)
        //    {
        //        int x1 = rnd.Next(150);
        //        int y1 = rnd.Next(30);
        //        int x2 = rnd.Next(150);
        //        int y2 = rnd.Next(30);
        //        Color clr = color[rnd.Next(color.Length)];
        //        g.DrawLine(new Pen(clr), x1, y1, x2, y2);
        //    }


        //    //画验证码字符串
        //    for (int i = 0; i < chkCode.Length; i++)
        //    {
        //        string fnt = font[rnd.Next(font.Length)];
        //        Font ft = new Font(fnt, 16);
        //        Color clr = color[rnd.Next(color.Length)];
        //        g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 20, (float)6);
        //    }


        //    //画噪点
        //    for (int i = 0; i < 100; i++)
        //    {
        //        int x = rnd.Next(bmp.Width);
        //        int y = rnd.Next(bmp.Height);
        //        Color clr = color[rnd.Next(color.Length)];
        //        bmp.SetPixel(x, y, clr);
        //    }

        //    //清除该页输出缓存，设置该页无缓存
        //    HttpContext.Current.Response.Buffer = true;
        //    HttpContext.Current.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
        //    HttpContext.Current.Response.Expires = 0;
        //    HttpContext.Current.Response.CacheControl = "no-cache";


        //    //将验证码图片写入内存流，并将其以“image/Png”格式输出
        //    MemoryStream ms = new MemoryStream();
        //    try
        //    {
        //        bmp.Save(ms, ImageFormat.Png);
        //        HttpContext.Current.Response.ClearContent();
        //        HttpContext.Current.Response.ContentType = "image/Png";
        //        HttpContext.Current.Response.BinaryWrite(ms.ToArray());
        //    }
        //    finally
        //    {
        //       //显示释放资源
        //        bmp.Dispose();
        //        g.Dispose();
        //    }

        //}
        #endregion

        #region First Method

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="ctype"></param>
        public virtual void getImageValidate(CodeType ctype)
        {
            //string str = "OO00"; //前两个为字母O，后两个为数字0
            string strValue = string.Empty;

            switch (ctype)
            {
                case CodeType.Num:
                    strValue = Common.CodeCommon.CreateCode(Common.CodeCommon.CodeType.Num);
                    break;
                case CodeType.English:
                    strValue = Common.CodeCommon.CreateCode(Common.CodeCommon.CodeType.English);
                    break;
                case CodeType.Chinese:
                    strValue = Common.CodeCommon.CreateCode(Common.CodeCommon.CodeType.Chinese);
                    break;
                case CodeType.All:
                    strValue = Common.CodeCommon.CreateCode(Common.CodeCommon.CodeType.All);
                    break;
                default:
                    strValue = Common.CodeCommon.CreateCode(Common.CodeCommon.CodeType.All);
                    break;
            }
            vcode = strValue;

            //颜色列表，用于验证码，噪线，噪点
            Color[] color = { Color.FromArgb(200, 191, 231), Color.FromArgb(239, 228, 176), Color.FromArgb(239, 228, 176), Color.FromArgb(149, 223, 255), Color.FromArgb(169, 243, 160) };
            int width = 0;
            if (ctype == CodeType.Chinese)
            {
                width = Convert.ToInt32(strValue.Length * 26);    //计算图像宽度
            }
            else
            {
                width = Convert.ToInt32(strValue.Length * 17);    //计算图像宽度
            }
            Bitmap img = new Bitmap(width, 30);
            Graphics gfc = Graphics.FromImage(img);           //产生Graphics对象，进行画图


            //gfc.Clear(Color.White);

            //绘制渐变背景
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            Brush brushBack = new LinearGradientBrush(rect, Color.FromArgb(rd.Next(120, 256), 255, 255), Color.FromArgb(255, rd.Next(120, 256), 255), rd.Next(180));
            gfc.FillRectangle(brushBack, rect);



            //画噪线，让噪线在验证码底层
            drawLine(gfc, img, color);

            //画噪点
            drawPoint(img);

            var valuelength = Convert.ToInt32(img.Width / strValue.Length);
            var valueheight = (img.Height / 3) * 2;
            //var i = 0;

            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;


            //字体列表，用于验证码
            string[] font_arr = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };

            //float l = 0;

            //进行单个字符位置随机
            foreach (var item in strValue)
            {
                Font font = new Font(font_arr[rd.Next(font_arr.Length)], 16, FontStyle.Bold);
                System.Drawing.Drawing2D.LinearGradientBrush brush =
                    new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, img.Width, img.Height), Color.DarkOrchid, Color.Blue, 1.5f, true);

                Point dot = new Point(14, 14);
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                float angle = rd.Next(-45, 45);//转动的度数

                gfc.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                gfc.RotateTransform(angle);
                gfc.DrawString(item.ToString(), font, brush, 1, 1, format);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                gfc.RotateTransform(-angle);//转回去
                gfc.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别

                #region  旧版

                //string s = new string(item, 1);

                //角度随机
                //var angle = rd.Next(1, 15);

                //Font font = new Font(font_arr[rd.Next(font_arr.Length)], 16, FontStyle.Bold);
                //System.Drawing.Drawing2D.LinearGradientBrush brush =
                //    new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, img.Width, img.Height), Color.DarkOrchid, Color.Blue, 1.5f, true);

                //SizeF size = gfc.MeasureString(s, font);


                ////设置旋转中心为文字中心
                //gfc.TranslateTransform(l + size.Width / 2, size.Height / 2);

                //旋转
                //gfc.RotateTransform((float)(rd.NextDouble() * angle * 2 - angle),MatrixOrder.Prepend);

                //gfc.RotateTransform(angle);

                //位置随机
                //gfc.DrawString(item.ToString(), font, brush, (5 + (10 * i)), 2);

                //gfc.DrawString(item.ToString(), font, brush, (rd.Next(2, 7) + (10 * i)), rd.Next(1, 10));
                //i++;
                #endregion
            }



            ////画噪点
            //drawPoint(img);


            //文字对齐方式
            //StringFormat sf = new StringFormat();
            //var LineRan = rd.Next(1, 2);
            //switch (LineRan)
            //{
            //    case 1:
            //        sf.LineAlignment = StringAlignment.Center;
            //        break;
            //    case 2:
            //        sf.LineAlignment = StringAlignment.Far;
            //        break;
            //    case 3:
            //        sf.LineAlignment = StringAlignment.Near;
            //        break;
            //    default:
            //        break;
            //}

            //var AlignRan = rd.Next(1, 2);
            //switch (AlignRan)
            //{
            //    case 1:
            //        sf.Alignment = StringAlignment.Center;
            //        break;
            //    case 2:
            //        sf.Alignment = StringAlignment.Far;
            //        break;
            //    case 3:
            //        sf.Alignment = StringAlignment.Near;
            //        break;
            //    default:
            //        break;
            //}


            //gfc.DrawRectangle(new Pen(Color.DarkBlue), 0, 0, img.Width - 1, img.Height - 1);
            //将图像添加到页面
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            //更改Http头
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //Dispose
            gfc.Dispose();
            img.Dispose();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 画噪线
        /// </summary>
        /// <param name="gfc"></param>
        /// <param name="img"></param>
        /// <param name="color"></param>
        private void drawLine(Graphics gfc, Bitmap img, Color[] color)
        {
            //选择画10条线,也可以增加，也可以不要线，只要随机杂点即可
            for (int i = 0; i < 10; i++)
            {
                int x1 = rd.Next(img.Width);
                int y1 = rd.Next(img.Height);
                int x2 = rd.Next(img.Width);
                int y2 = rd.Next(img.Height);
                gfc.DrawLine(new Pen(color[rd.Next(color.Length)]), x1, y1, x2, y2);      //注意画笔一定要浅颜色，否则验证码看不清楚
            }
        }

        /// <summary>
        /// 画噪点
        /// </summary>
        /// <param name="img"></param>
        private void drawPoint(Bitmap img)
        {
            for (int i = 0; i < 100; i++)
            {
                int col = rd.Next();//在一次的图片中杂店颜色相同
                int x = rd.Next(img.Width);
                int y = rd.Next(img.Height);
                img.SetPixel(x, y, Color.FromArgb(col));
            }
        }

        #endregion
    }
}
