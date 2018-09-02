using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalREvaulation.Contracts.Models;

namespace SignalRWebAPI.Hubs
{
    public class PersonHub : Hub<IPersonClientHub>
    {
        public async Task Add(Person person) => await Clients.All.AddPerson(person);
        public async Task Delete(long personId) => await Clients.All.DeletePerson(personId);
        public async Task Change(Person person) => await Clients.All.Change(person);
    }
}