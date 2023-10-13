using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;

namespace CSharpTutorial
{
    public class Program
    {
        //引用变量
        public static void Swap(ref int num1, ref int num2)
        {
            int temp = num1;
            num1 = num2;
            num2 = temp;
        }

        //**params：**可以不限制地传入参数
        public static double GetSumMore(params double[] nums)
        {
            double sum = 0;
            foreach (double num in nums)
            {
                sum += num;
            }
            return sum;
        }

        //使用定义的参数就可以无需按需传入参数
        static void PrintInfo(string name, int zipCode)
        {
            Console.WriteLine("{0} lives in the zip code {1}", name, zipCode);
        }

        //函数重载
        static double GetSum2(double x = 1, double y = 1)
        {
            return x + y;
        }

        static double GetSum2(string x = "1", string y = "1")
        {
            double dblX = Convert.ToDouble(x);
            double dblY = Convert.ToDouble(y);
            return dblX + dblY;
        }


        static void Main(string[] args)
        {
            //**ref：**可以引用变量
            int num = 0;
            int num2 = 1;
            Console.WriteLine("Before Swap num1 : {0} num2 : {1}", num, num2);
            Swap(ref num, ref num2);
            Console.WriteLine("After Swap num1 : {0} num2 : {1}", num, num2);

            //**params：**可以不限制地传入参数
            Console.WriteLine("1 + 2 + 3 + 4 + 5 + 6 = {0} ", GetSumMore(1, 2, 3, 4, 5, 6));

            //使用定义的参数就可以无需按需传入参数
            PrintInfo(zipCode: 15147, name: "Derek Banas");

            //**函数重载**
            Console.WriteLine("5.0 + 4.0 = {0}", GetSum2(5.0, 4.5));
            Console.WriteLine("5 + 4 = {0}", GetSum2(5, 4));
            Console.WriteLine("5 + 4 = {0}", GetSum2("5", "4"));

        }

    }
}