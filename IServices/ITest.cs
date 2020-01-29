using System;
using MessagePack;

namespace IServices
{
    public interface ITest
    {
        string Get(string input);

        Person GetPerson(int id);
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
        [Key(4)]
        public Person Per { get; set; }
    }
}
