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
        static void Main(string[] args)
        {
            Arithmetic add, sub, addSub;
            add = new Arithmetic(Add);
            sub = new Arithmetic(Subtract);
            addSub = add + sub;

            Console.WriteLine($"Add {6} & {10}");
            add(6, 10);

            Console.WriteLine($"Add & Subtract {10} & {4}");
            addSub(10, 4);
        }

        public delegate void Arithmetic(double num1, double num2);

        public static void Add(double num1, double num2)
        {
            Console.WriteLine($"{num1} + {num2} = {num1 + num2}");
        }

        public static void Subtract(double num1, double num2)
        {
            Console.WriteLine($"{num1} - {num2} = {num1 - num2}");
        }

    }
}