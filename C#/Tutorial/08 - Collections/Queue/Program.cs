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
            #region Queue Code

            Queue queue = new Queue();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Console.WriteLine("1 in Queue : {0}", queue.Contains(1));

            Console.WriteLine("Remove 1 : {0}", queue.Dequeue());

            Console.WriteLine("Peek 1 : {0}", queue.Peek());

            object[] numArray = queue.ToArray();

            Console.WriteLine(string.Join(",", numArray));

            foreach (object o in queue)
            {
                Console.WriteLine($"Queue : {o}");
            }

            queue.Clear();


            #endregion


        }

    }
}