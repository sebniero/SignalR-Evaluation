using System.Collections.Generic;
using SignalREvaulation.Contracts.Models;

namespace SignalRWebAPI.LocalStorage
{
    public interface IPersonService
    {
        Person AddPerson(Person newPerson);
        void DeletePerson(long personId);
        void DeletePerson(Person person);
        Person Find(long id);
        IEnumerable<Person> GetAllPersons();
        Person UpdatePerson(Person person);
    }
}