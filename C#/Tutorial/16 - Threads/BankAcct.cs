using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    internal class BankAcct
    {
        private Object acctLock = new Object();
        public double Balance { get; set; }
        public string Name { get; set; }

        public BankAcct(double balance)
        {
            Balance = balance;
        }

        public double Withdraw(double amount)
        {
            if(Balance - amount < 0)
            {
                Console.WriteLine($"Sorry ${Balance} in Account");
                return Balance;
            }
            lock(acctLock)
            {
                if(Balance >= amount)
                {
                    Console.WriteLine("Remove {0} and {1} left in account", amount, (Balance - amount));
                    Balance -= amount;
                }
                return Balance;
            }
        }

        public void IssueWithdraw()
        {
            Withdraw(1);
        }

    }
}
