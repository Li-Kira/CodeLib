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
            AnimalFarm animalFarm = new AnimalFarm();
            animalFarm[0] = new Animal("Cat");
            animalFarm[1] = new Animal("Dog");
            animalFarm[2] = new Animal("Bird");
            animalFarm[3] = new Animal("Duck");

            foreach(Animal animal in animalFarm)
            {
                Console.WriteLine(animal.Name);
            }
        }


    }
}