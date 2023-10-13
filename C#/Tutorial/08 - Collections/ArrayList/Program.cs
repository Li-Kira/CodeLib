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
            #region ArrayList Code

            ArrayList aList = new ArrayList();

            aList.Add("Bob");
            aList.Add(40);

            Console.WriteLine("Count: {0}", aList.Count);
            Console.WriteLine("Capacity: {0}", aList.Capacity);

            ArrayList aList2 = new ArrayList();

            aList2.AddRange(new object[] { "Mike", "Sally", "Egg" });
            aList.AddRange(aList2);


            aList2.Sort();
            aList2.Reverse();

            aList2.Insert(1, "Turkey");

            ArrayList range = aList2.GetRange(0, 2);

            Console.WriteLine("Turkey Index : {0}", aList2.IndexOf("Turkey", 0));

            foreach (object o in range)
            {
                Console.WriteLine(o);
            }

            string[] myArray = (string[])aList2.ToArray(typeof(string));
            string[] customers = { "Bob", "Sally", "Sue" };
            ArrayList custArrayList = new ArrayList();
            custArrayList.AddRange(customers);

            #endregion



        }

    }
}