using System;
using System.Collections.Generic;
using MessagePack;

namespace IServices
{
    public interface ITest
    {
        Person GetPerson(int id);

        int GetCount(List<Person> persons);
    }


    [MessagePackObject]
    public class Person
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public decimal Money { get; set; }
        [Key(3)]
        public DateTime Date { get; set; }
    }
}
