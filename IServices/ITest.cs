using System;
using System.Collections.Generic;

namespace IServices
{
    public interface ITest
    {
        string Get(string input);

        Person GetPerson(int id);
    }


    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Money { get; set; }

        public DateTime Date { get; set; }

        public Person Per { get; set; }

        public List<Person> Persons { get; set; }
    }
}
