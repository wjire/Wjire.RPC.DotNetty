using System;
using System.Collections.Generic;
using System.ServiceModel;
using MessagePack;

namespace IServices
{
    [ServiceContract]
    public interface ITest
    {
        Person GetPerson(int id);

        int GetCount(List<Person> persons);

        string GetString();
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
