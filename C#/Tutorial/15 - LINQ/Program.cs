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
            Animal[] animalList = new[] 
            {
                new Animal{ Name = "German Shepherd", Height = 25, Weight = 77, AnimalID = 1 },
                new Animal{ Name = "Chihuahua", Height = 7, Weight = 4.4, AnimalID = 2 },
                new Animal{ Name = "Saint Bernard", Height = 30, Weight = 200, AnimalID = 3 },
                new Animal{ Name = "Pug", Height = 12, Weight = 16, AnimalID = 1 },
                new Animal{ Name = "Beagle", Height = 15, Weight = 23, AnimalID =  2 }
            };

            var animalEnum = animalList.OfType<Animal>();

            var selectedAnimal = from animal in animalEnum
                                 where animal.Weight <= 90
                                 orderby animal.Name
                                 select animal;

            foreach(var animal in selectedAnimal)
            {
                Console.WriteLine("{0} weight {1}lbs", animal.Name, animal.Weight);
            }
            Console.WriteLine();

            Owner[] owners = new[]
           {
                new Owner{ Name = "Doug Parks", OwnerID = 1 },
                new Owner{ Name = "Sally Smith", OwnerID = 2 },
                new Owner{ Name = "Paul Brooks", OwnerID = 3 }
            };

            var nameHeight = from a in animalList
                             select new
                             {
                                 a.Name,
                                 a.Height
                             };

            Array arrNameHeight = nameHeight.ToArray();

            foreach (var i in arrNameHeight)
            {
                Console.WriteLine(i.ToString());
            }
            Console.WriteLine();

            var innerJoin = from animal in animalList
                            join owner in owners on animal.AnimalID
                            equals owner.OwnerID
                            select new { OwnerName = owner.Name, AnimalName = animal.Name };

            foreach (var i in innerJoin)
            {
                Console.WriteLine("{0} owns {1}",
                    i.OwnerName, i.AnimalName);
            }

            Console.WriteLine();

            var groupJoin = from owner in owners
                            orderby owner.OwnerID
                            join animal in animalList on owner.OwnerID
                            equals animal.AnimalID into ownerGroup
                            select new
                            {
                                Owner = owner.Name,
                                Animals = from owner2 in ownerGroup
                                          orderby owner2.Name
                                          select owner2
                            };

            int totalAnimals = 0;

            foreach (var ownerGroup in groupJoin)
            {
                Console.WriteLine(ownerGroup.Owner);
                foreach (var animal in ownerGroup.Animals)
                {
                    totalAnimals++;
                    Console.WriteLine("* {0}", animal.Name);
                }
            }

        }


    }
}