//#define EXAMPLE_1
//#define EXAMPLE_2
#define EXAMPLE_3
//#define EXAMPLE_4

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
#if EXAMPLE_1

        static void Print1()
        {
            for(int i = 0; i < 1000; i++)
            {
                Console.Write(1);
            }
        }
#elif EXAMPLE_4
        static void CountTo(int maxNum)
        {
            for (var i = 0; i < maxNum; i++)
            {
                Console.WriteLine(i);
            }
        }
#endif


        static void Main(string[] args)
        {
#if EXAMPLE_1
            Thread thread = new Thread(Print1);
            thread.Start();
            for (int i = 0; i < 1000; i++)
            {
                Console.Write(0);
            }
#elif EXAMPLE_2
            int num = 1;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(num);
                Thread.Sleep(1000);
                num++;
            }
            Console.WriteLine("Thread Ends");

#elif EXAMPLE_3
            BankAcct acct = new BankAcct(10);
            Thread[] threads = new Thread[15];

            Thread.CurrentThread.Name = "main";

            for(int i = 0; i < 15; i++)
            {
                Thread thread = new Thread(new ThreadStart(acct.IssueWithdraw));
                thread.Name = i.ToString();
                threads[i] = thread;
            }

            for (int i = 0; i < 15; i++)
            {
                Console.WriteLine("Thread {0} Alive : {1} ", threads[i].Name, threads[i].IsAlive);
                threads[i].Start();
                Console.WriteLine("Thread {0} Alive : {1} ", threads[i].Name, threads[i].IsAlive);
            }

            Console.WriteLine("Current Priority {0}", Thread.CurrentThread.Priority);
            Console.WriteLine("Thread {0} Ending", Thread.CurrentThread.Name);


#elif EXAMPLE_4
            Thread thread = new Thread(() => CountTo(10));
            thread.Start();

            new Thread(() =>
            {
                CountTo(5);
                CountTo(6);
            }).Start();

#endif

        }


    }
}