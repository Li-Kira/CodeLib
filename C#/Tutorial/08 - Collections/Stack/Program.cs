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
            #region Stack Code

            Stack stack = new Stack();

            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Console.WriteLine("Peek 1 : {0}", stack.Peek());

            Console.WriteLine("Pop 1 : {0}", stack.Pop());

            Console.WriteLine("Contain 1 : {0}", stack.Contains(1));

            object[] numArray2 = stack.ToArray();

            Console.WriteLine(string.Join(",", numArray2));

            foreach (object o in stack)
            {
                Console.WriteLine($"Stack : {o}");
            }
            #endregion


        }

    }
}