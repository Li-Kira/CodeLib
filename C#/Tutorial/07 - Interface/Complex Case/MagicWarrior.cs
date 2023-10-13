using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    class MagicWarrior : Warrior
    {
        int teleportChance = 0;
        CanTeleport teleportType = new CanTeleport();
        public MagicWarrior(string name, double health, double attckMax, double blockMax, int teleportChance):base(name, health, attckMax, blockMax)
        {
            this.teleportChance = teleportChance;
        }

        public override double Block()
        {
            Random random = new Random();
            int randDodge = random.Next(1, 100);

            if (randDodge < this.teleportChance)
            {
                Console.WriteLine($"{Name} {teleportType.teleport()}");
                return 10000;
            }
            else
            {
                return base.Block();
            }
           
        }

    }
}
