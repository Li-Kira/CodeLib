//#define EXAMPLE_1
//#define EXAMPLE_2
#define EXAMPLE_3

using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace CSharpTutorial
{
    public class Program
    {
        static void Main(string[] args)
        {
#if EXAMPLE_1
            Animal bowser = new Animal("Bowser", 45, 25, 1);
            Stream stream = File.Open("AnimalData.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream, bowser);
            stream.Close();

            bowser = null;

            stream = File.Open("AnimalData.dat", FileMode.Open);
            bf = new BinaryFormatter();
            bowser = (Animal)bf.Deserialize(stream);
            stream.Close();

            Console.WriteLine(bowser.ToString());
        
#elif EXAMPLE_2
            DirectoryInfo dataDir = new DirectoryInfo(@"E:\Code\Course\C#\CSharpTutorial\CSharpData");
            if(!dataDir.Exists)
                dataDir.Create();

            Animal bowser = new Animal("Bowser", 45, 25, 1);
            XmlSerializer serializer = new XmlSerializer(typeof(Animal));
            using (TextWriter writer = new StreamWriter(@"E:\Code\Course\C#\CSharpTutorial\CSharpData\bowser.xml"))
            {
                serializer.Serialize(writer, bowser);
            }
            bowser = null;

            XmlSerializer deserializer = new XmlSerializer(typeof(Animal));
            TextReader reader = new StreamReader(@"E:\Code\Course\C#\CSharpTutorial\CSharpData\bowser.xml");
            object obj = deserializer.Deserialize(reader);
            bowser = (Animal)obj;
            reader.Close();

            Console.WriteLine(bowser.ToString());

#elif EXAMPLE_3
            DirectoryInfo dataDir = new DirectoryInfo(@"E:\Code\Course\C#\CSharpTutorial\CSharpData");
            if (!dataDir.Exists)
                dataDir.Create();

            //Write List
            List<Animal> theAnimals = new List<Animal>
            {
                new Animal("Mario", 60, 30, 2),
                new Animal("Luigi", 55, 24, 3),
                new Animal("Peach", 40, 20, 4)
            };

            using (Stream fs = new FileStream(@"E:\Code\Course\C#\CSharpTutorial\CSharpData\bowser.xml", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer2 = new XmlSerializer(typeof(List<Animal>));
                serializer2.Serialize(fs, theAnimals);
            }

            theAnimals = null;

            XmlSerializer serializer3 = new XmlSerializer(typeof(List<Animal>));

            using (FileStream fs2 = File.OpenRead(@"E:\Code\Course\C#\CSharpTutorial\CSharpData\bowser.xml"))
            {
                theAnimals = (List<Animal>)serializer3.Deserialize(fs2);
            }


            foreach (Animal a in theAnimals)
            {
                Console.WriteLine(a.ToString());
            }

#endif
        }
    }
}