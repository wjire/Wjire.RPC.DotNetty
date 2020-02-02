using System;
using System.Collections.Generic;
using IServices;

namespace Services
{
    public class Test : ITest
    {
        public Person GetPerson(int id)
        {
            return new Person
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Money = 12000M,
            };
        }

        public int GetCount(List<Person> persons)
        {
            return persons.Count;
        }
    }
}
