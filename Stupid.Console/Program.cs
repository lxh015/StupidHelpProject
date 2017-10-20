using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stupid.TConsole
{
    class Program
    {



        static void TMain(string[] args)
        {
            Work work = new Work();

            Price price = new Price();

            price.change += new Price.ChangePrice(work.ShowPrice);

            price.Change(1000);



            Task.Run(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("stets");
                
            });


            Console.WriteLine("请输入字：");
            var a=Console.ReadLine();
            Console.WriteLine(a);


            Action<string, string> deleage = new Action<string, string>(testTask);

            deleage.Invoke("1111", "2222");



            Console.WriteLine("tests111111");

            Console.ReadKey();

        }

        public static void testTask(string ddd,string ddd2)
        {
            Console.WriteLine("{0}-{1}",ddd,ddd2);
        }






        public class Price
        {
            public double price = 500000;


            public delegate void ChangePrice(double price);

            public event ChangePrice change;


            public void Change(double price)
            {

                Console.WriteLine("p price {0}", this.price);
                this.price = price * 2;
                Console.WriteLine("p price {0}", this.price);

                change(this.price);
            }
        }


        public class Work
        {
            public double price = 500;


            public Work()
            {
                Console.WriteLine("now price {0}", this.price);
            }

            public void ShowPrice(double price)
            {
                this.price = price * 100;
                Console.WriteLine("now price {0}", this.price);
            }
        }
    }
}
