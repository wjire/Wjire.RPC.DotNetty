using System;
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
                Id = new Random().Next(1, int.MaxValue),
                Name = Guid.NewGuid().ToString()
            };
        }
    }
}
