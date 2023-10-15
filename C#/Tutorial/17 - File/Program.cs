//#define EXAMPLE_1
//#define EXAMPLE_2
//#define EXAMPLE_3
#define EXAMPLE_4
//#define EXAMPLE_5

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
#if EXAMPLE_1
            DirectoryInfo currentDir = new DirectoryInfo(".");
            
            Console.WriteLine(currentDir.FullName);
            Console.WriteLine(currentDir.Name);
            Console.WriteLine(currentDir.Parent);
            Console.WriteLine(currentDir.Attributes);
            Console.WriteLine(currentDir.CreationTime);

            DirectoryInfo dataDir = new DirectoryInfo(@"E:\Code\Course\C#\CSharpTutorial\CSharpData");
            dataDir.Create();
            Console.WriteLine("Data Dir is Exists? : {0}", dataDir.Exists);
            
            //dataDir.Delete();

#elif EXAMPLE_2
            string[] customers =
            {
                "Bob Smith",
                "Sally Smith",
                "Robert Smith"
            };

            string textFilePath = @"E:\Code\Course\C#\CSharpTutorial\CSharpData\testfile1.txt";
            File.WriteAllLines(textFilePath, customers);

            foreach (string cust in File.ReadAllLines(textFilePath))
            {
                Console.WriteLine($"Customer : {cust}");
            }

            DirectoryInfo myDataDir = new DirectoryInfo(@"E:\Code\Course\C#\CSharpTutorial\CSharpData");
            FileInfo[] txtFiles = myDataDir.GetFiles("*.txt", SearchOption.AllDirectories);

            Console.WriteLine("Matches : {0}", txtFiles.Length);

            foreach (FileInfo file in txtFiles)
            {
                Console.WriteLine(file.Name);
                Console.WriteLine(file.Length);
            }
#elif EXAMPLE_3
            string textFilePath = @"E:\Code\Course\C#\CSharpTutorial\CSharpData\testfile1.txt";
            FileStream fileStream = File.Open(textFilePath, FileMode.Create);

            string randString = "This is a random string";
            byte[] randBytes = Encoding.Default.GetBytes(randString);
            fileStream.Write(randBytes, 0, randBytes.Length);
            fileStream.Position = 0;

            byte[] fileByteArray = new byte[randBytes.Length];

            for(int i = 0; i < randBytes.Length; i++)
            {
                fileByteArray[i] = (byte)fileStream.ReadByte();
            }
            Console.WriteLine(Encoding.Default.GetString(fileByteArray));
            fileStream.Close();
#elif EXAMPLE_4
            string textFilePath = @"E:\Code\Course\C#\CSharpTutorial\CSharpData\testfile1.txt";
            StreamWriter sw = new StreamWriter(textFilePath);
            sw.Write("This is a random ");
            sw.WriteLine("sentence.");
            sw.WriteLine("This is another sentence.");
            sw.Close();

            StreamReader sr = new StreamReader(textFilePath);
            Console.WriteLine("Peek : {0}", Convert.ToChar(sr.Peek()));
            Console.WriteLine("1st String : {0}", sr.ReadLine());
            Console.WriteLine("Everything Else : {0}", sr.ReadToEnd());
            sr.Close();
#elif EXAMPLE_5
            string textFilePath = @"E:\Code\Course\C#\CSharpTutorial\CSharpData\testfile1.txt";
            FileInfo fileInfo = new FileInfo(textFilePath);
            BinaryWriter bw = new BinaryWriter(fileInfo.OpenWrite());
            string randText = "Random Text";
            int myAge = 0;
            double height = 6.25;
            bw.Write(randText);
            bw.Write(myAge);
            bw.Write(height);
            bw.Close();

            BinaryReader br = new BinaryReader(fileInfo.OpenRead());
            Console.WriteLine(br.ReadString());
            Console.WriteLine(br.ReadInt32());
            Console.WriteLine(br.ReadDouble());
            br.Close();

#endif

        }


    }
}