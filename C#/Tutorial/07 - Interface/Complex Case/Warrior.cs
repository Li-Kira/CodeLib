using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    class Warrior
    {
        public string Name { get; set; }
        public double Health { get; set; }
        public double AttckMax { get; set; }
        public double BlockMax { get; set; }

        public Warrior(string name, double health, double attckMax, double blockMax)
        {
            Name = name;
            Health = health;
            AttckMax = attckMax;
            BlockMax = blockMax;
        }

        Random random = new Random();
        public double Attack()
        {
            return random.Next(1, (int)AttckMax);
        }

        public virtual double Block()
        {
            return random.Next(1, (int)BlockMax);
        }


    }
}
