using System.Threading.Tasks;
using SignalREvaulation.Contracts.Models;

namespace SignalRWebAPI.Hubs
{
    public interface IPersonClientHub
    {
        Task AddPerson(Person person);
        Task DeletePerson(long personId);
        Task Change(Person person);
    }
}