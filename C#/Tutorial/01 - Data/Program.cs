using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;

namespace CSharpTutorial
{
    public class Program
    {
        static void PrintArray(int[] intArray, string mess)
        {
            foreach (int k in intArray)
            {
                Console.WriteLine("{0} : {1} ", mess, k);
            }
        }

        static void Main(string[] args)
        {
            bool boolFromStr = bool.Parse("true");
            int intFromStr = int.Parse("123");
            double dblFromStr = double.Parse("3.1415");

            /*--------------------------------------------------------------------*/

            string str = dblFromStr.ToString();

            /*--------------------------------------------------------------------*/

            //货币 
            //Currency : ￥23.45
            Console.WriteLine("Currency : {0:c}", 23.455);
            //向前填充 
            //Pad with 0s : 0023
            Console.WriteLine("Pad with 0s : {0:d4}", 23);
            //精确到三位小数
            //3 Decimals : 23.456
            Console.WriteLine("3 Decimals : {0:f3}", 23.45555);
            //逗号隔开
            //Commmas : 2,300.0000
            Console.WriteLine("Commmas : {0:n4} ", 2300);
            //逗号隔开
            //Commmas : 230,000,000
            Console.WriteLine("Commmas : {0:n0} ", 230000000);

            /*--------------------------------------------------------------------*/

            string randString = "This is a string";
            Console.WriteLine("String Length : {0}", randString.Length);
            Console.WriteLine("String Contains is : {0}", randString.Contains("is"));
            Console.WriteLine("Index of is : {0}", randString.IndexOf("is"));
            Console.WriteLine("Remove String : {0}", randString.Remove(10, 6));
            Console.WriteLine("Insert String : {0}", randString.Insert(10, "short "));
            Console.WriteLine("Replace String : {0}", randString.Replace("string", "sentence"));
            Console.WriteLine("Compare A to B : {0}", String.Compare("A", "B", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("-------------------");

            Console.WriteLine("A = a : {0}", String.Equals("A", "a", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine("Pad Left : {0}", randString.PadLeft(20, '.'));
            Console.WriteLine("Pad Right : {0}", randString.PadRight(20, '.'));
            // Trim whitespace
            Console.WriteLine("Trim : {0}", randString.Trim());
            Console.WriteLine("Uppercase : {0}", randString.ToUpper());
            Console.WriteLine("Lowercase : {0}", randString.ToLower());
            string newString = String.Format("{0} saw a {1} {2} in the {3}", "Paul", "rabbit", "eating", "field");
            Console.Write(newString + "\n");

            Console.WriteLine(@"Exactly what I type\n");

            /*--------------------------------------------------------------------*/

            int[] favNums = new int[3];
            favNums[0] = 23;
            Console.WriteLine("favNum 0 : {0}", favNums[0]);
            string[] customers = { "Bob", "Sally", "Sue" };
            var employees = new[] { "Mike", "Paul", "Rick" };
            object[] randomArray = { "Paul", 45, 1.234 };

            Console.WriteLine("randomArray 0 : {0}", randomArray[0].GetType());
            Console.WriteLine("Array Size : {0}", randomArray.Length);

            for (int j = 0; j < randomArray.Length; j++)
            {
                Console.WriteLine("Array : {0}: Value : {1}", j, randomArray[j]);
            }

            Console.WriteLine("---------------------------");

            string[,] custNames = new string[2, 2] { { "Bob", "Sam" }, { "Sally", "Smith" } };
            Console.WriteLine("MD Value : {0}", custNames.GetValue(1, 0));

            for (int j = 0; j < custNames.GetLength(0); j++)
            {
                for (int k = 0; k < custNames.GetLength(0); k++)
                {
                    Console.WriteLine("{0} ", custNames[j, k]);
                }
                Console.WriteLine();
            }

            int[] randNums = { 3, 4, 1, 2 };
            PrintArray(randNums, "ForEach");

            Console.WriteLine("---------------------------");

            Array.Sort(randNums);
            Array.Reverse(randNums);
            Console.WriteLine("1 at index: {0}", Array.IndexOf(randNums, 1));
            randNums.SetValue(0, 1);
            int[] srcArray = { 1, 2, 3 };
            int[] desArray = new int[2];
            int startId = 0;
            int length = 2;
            Array.Copy(srcArray, startId, desArray, 0, length);
            PrintArray(desArray, "Copy");

            Array anotherArray = Array.CreateInstance(typeof(int), 10);
            srcArray.CopyTo(anotherArray, 5);

            foreach (int m in anotherArray)
            {
                Console.WriteLine("CopyTo : {0} ", m);
            }
        }
    }
}