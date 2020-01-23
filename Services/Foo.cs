using System;
using System.Collections.Generic;
using System.Text;
using IServices;

namespace Services
{
    public class Foo : IFoo
    {
        public Person Get()
        {
            return new Person
            {
                Date = DateTime.Now,
                Id = new Random().Next(1, Int32.MaxValue),
                Name = Guid.NewGuid().ToString()
            };
        }
    }
}
