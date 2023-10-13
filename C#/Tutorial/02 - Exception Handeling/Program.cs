using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;

namespace CSharpTutorial
{
    public class Program
    {
        static double Division_fn(double x, double y)
        {
            if (y == 0)
            {
                throw new System.DivideByZeroException();
            }

            return x / y;
        }
        static void Main(string[] args)
        {
            double num1 = 5;
            double num2 = 0;

            try
            {
                Console.WriteLine("5 / 0 = {0}", Division_fn(num1, num2));

            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine(e.GetType().Name);
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}