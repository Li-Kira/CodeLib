using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace CSharpTutorial
{
    public class Program
    {
        delegate double doubleIt(double x);

        static void Main(string[] args)
        {
            var shopkins = new { Name = "Shopkins", Price = 4.99 };
            Console.WriteLine("{0} cost ${1}", shopkins.Name, shopkins.Price);

            var toyArray = new[] 
            { 
                new { Name = "Yo-Kai Pack", Price = 4.99 }, 
                new { Name = "Legos", Price = 9.99 } 
            };


            foreach (var item in toyArray)
            {
                Console.WriteLine("{0} costs ${1}", item.Name, item.Price);
            }

        }


    }
}