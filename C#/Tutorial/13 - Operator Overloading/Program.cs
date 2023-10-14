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
            Box box1 = new Box(2, 3, 4);
            Box box2 = new Box(5, 6, 7);
            Box box3 = box1 + box2;

            Console.WriteLine($"Box 3 : {box3}");
            Console.WriteLine($"Box int : {(int)box3}");
            Box box4 = (Box)4;
            Console.WriteLine($"Box 4 : {box4}");

        }


    }
}