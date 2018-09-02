using System.Collections.Generic;
using System.Linq;
using SignalREvaulation.Contracts.Models;

namespace SignalRWebAPI.LocalStorage
{
    public class PersonService : IPersonService
    {
        private long _lastId = 0;
        private readonly List<Person> _persons = new List<Person>();

        public IEnumerable<Person> GetAllPersons()
        {
            return _persons;
        }

        public Person UpdatePerson(Person person)
        {
            var foundPerson = FindPersonById(person.Id);

            foundPerson.BirthDate = person.BirthDate;
            foundPerson.BodySize = person.BodySize;
            foundPerson.Name = person.Name;

            return foundPerson;
        }

        public Person AddPerson(Person newPerson)
        {
            _lastId++;
            newPerson.Id = _lastId;
            _persons.Add(newPerson);

            return newPerson;
        }

        public void DeletePerson(Person person)
        {
            _persons.Remove(person);
        }

        public void DeletePerson(long personId)
        {
            var foundPerson = FindPersonById(personId);
            DeletePerson(foundPerson);
        }

        private Person FindPersonById(long personId)
        {
            var foundPerson = _persons.FirstOrDefault(person => person.Id == personId);
            return foundPerson;
        }

        public Person Find(long id)
        {
            return FindPersonById(id);
        }
    }
}