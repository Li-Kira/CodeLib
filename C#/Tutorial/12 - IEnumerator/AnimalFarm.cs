using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTutorial
{
    internal class AnimalFarm : IEnumerable
    {
        private List<Animal> m_AnimalList = new List<Animal>();

        public AnimalFarm(List<Animal> animal)
        {
            m_AnimalList = animal;
        }

        public AnimalFarm() { }

        public Animal this[int index]
        {
            get { return m_AnimalList[index]; }
            set { m_AnimalList.Insert(index, value); }
        }

        public int Count
        {
            get { return m_AnimalList.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return m_AnimalList.GetEnumerator();
        }
    }
}
