using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    internal class Animal
    {
        public string Name { get; set; }
        public Animal(string name)
        {
            Name = name;
        }

        public static void GetSum<T>(ref T num1, ref T num2)
        {
            double dblx = Convert.ToDouble(num1);
            double dbly = Convert.ToDouble(num2);
            Console.WriteLine($"{dblx} + {dbly} = {dblx + dbly}");
        }
    }
}
