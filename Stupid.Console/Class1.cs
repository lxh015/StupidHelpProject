using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stupid.TConsole
{
    class Class1
    {

        static void TMain()
        {
            //第一个委托人
            Implementation1 i1 = new Implementation1();
            //第二个委托人
            Implementation2 i2 = new Implementation2();
            //第三个委托人
            Implementation3 i3 = new Implementation3();

            //奸商来了！
            DeleageteT delegetes = new DeleageteT();

            //赶紧注册吧！
            delegetes.woshishijian += i1.show;
            delegetes.woshishijian += i2.show;
            delegetes.woshishijian += i3.show;

            //奸商开始干活了
            delegetes.StarDelegate("我的名字是：");

            Console.ReadKey();
        }

        /// <summary>
        /// 委托中间人
        /// </summary>
        public class DeleageteT
        {
            /// <summary>
            /// 定义一个委托，我能做的某一类事。
            /// </summary>
            /// <param name="text"></param>
            public delegate void woshiweituo(string text);

            /// <summary>
            /// 定义一个事件，谁要委托我啊！赶紧来！
            /// </summary>
            public event woshiweituo woshishijian;

            /// <summary>
            /// 定义一个开始方法，我可不知道什么时候开始干活，你要告诉哦！
            /// </summary>
            /// <param name="text"></param>
            public void StarDelegate(string text)
            {
                if (woshishijian != null)
                {
                    //执行注册中的事件。
                    woshishijian(text);
                }
                else
                {
                    Console.WriteLine("都没注册事件，托毛啊！");
                }
            }
        }

        /// <summary>
        /// 我是第一个委托人
        /// </summary>
        public class Implementation1
        {
            public void show(string text)
            {
                Console.WriteLine("{0}{1}", text, this.GetType().Name);
            }
        }

        /// <summary>
        /// 我是第二个委托人
        /// </summary>
        public class Implementation2
        {
            public void show(string text)
            {
                Console.WriteLine("{0}{1}", text, this.GetType().Name);
            }
        }

        /// <summary>
        /// 我是第三个委托人
        /// </summary>
        public class Implementation3
        {
            public void show(string text)
            {
                Console.WriteLine("{0}{1}", text, this.GetType().Name);
            }
        }
    }
}
