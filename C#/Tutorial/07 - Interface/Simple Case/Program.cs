﻿using CSharpTutorial;
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApp
{
    public class Program
    {

        static void Main(string[] args)
        {
            Vehicle buick = new Vehicle("Buick", 4, 160);

            if (buick is IDrivable)
            {
                buick.Move();
                buick.Stop();
            }
            else
            {
                Console.WriteLine("The {0} can't be driven", buick.Brand);
            }

            IElectronicDevice TV = TVRemote.GetDevice();

            PowerButton powBut = new PowerButton(TV);

            powBut.Execute();
            powBut.Undo();
            powBut.Execute();
            powBut.Undo();

        }

    }
}