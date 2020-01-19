using System;
using IServices;

namespace Services
{
    public class Test : ITest
    {
        public string Get(string input)
        {
            return input.ToUpper();
        }
        
        public Person GetPerson(int id)
        {
            return new Person
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Money = 12000M,
                //Per = new Person
                //{
                //    Id = 99,
                //    Name = Guid.NewGuid().ToString(),
                //    Date = DateTime.Now,
                //    Money = 12000M,
                //},
                //Persons = new System.Collections.Generic.List<Person>()
                //{
                //     new Person
                //     {
                //         Id = 1,
                //         Name = Guid.NewGuid().ToString(),
                //         Date = DateTime.Now,
                //         Money = 12000M,
                //     },
                //     new Person
                //     {
                //         Id = 2,
                //         Name = Guid.NewGuid().ToString(),
                //         Date = DateTime.Now,
                //         Money = 12000M,
                //     },
                //}
            };
        }
    }
}
