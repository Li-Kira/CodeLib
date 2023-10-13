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
            List<Animal> animalList = new List<Animal>();
            List<int> intList = new List<int>();

            intList.Add(24);
            animalList.Add(new Animal("Dog"));
            animalList.Add(new Animal("Cat"));
            animalList.Add(new Animal("Bird"));

            animalList.Insert(1, new Animal("Duck"));
            animalList.RemoveAt(1);

            Console.WriteLine("Num of Animals : {0}", animalList.Count);
            foreach (Animal animal in animalList)
            {
                Console.WriteLine(animal.Name);
            }

            int x = 5, y = 4;
            Animal.GetSum<int>(ref x, ref y);
            string strX = "5", strY = "4";
            Animal.GetSum<string>(ref strX, ref strY);

            Rectangle<int> rec1 = new Rectangle<int>(20, 50);
            Console.WriteLine(rec1.GetArea());

            Rectangle<string> rec2 = new Rectangle<string>("20", "50");
            Console.WriteLine(rec2.GetArea());

        }

        public struct Rectangle<T>
        {
            private T width;
            private T height;

            public T Width
            {
                get { return width; }
                set { width = value; }
            }

            public T Height
            {
                get { return height; }
                set { height = value; }
            }

            public Rectangle(T w, T h)
            {
                width = w;
                height = h;
            }

            public string GetArea()
            {
                double dlbWidth = Convert.ToDouble(Width);
                double dlbHeight = Convert.ToDouble(Height);
                return String.Format($"{Width} * {Height} = {dlbWidth * dlbHeight}");
            }

        }

    }
}