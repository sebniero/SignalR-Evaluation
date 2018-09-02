using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalREvaulation.Contracts.Models;
using SignalRWebAPI.Hubs;
using SignalRWebAPI.LocalStorage;

namespace SignalRWebAPI.Controllers
{
    public abstract class HubController<THub> : Controller
        where THub: Hub
    {
        public IHubContext<THub> HubContext { get; }

        protected HubController(IHubContext<THub> hubContext)
        {
            HubContext = hubContext;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : HubController<PersonHub>
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService, IHubContext<PersonHub> hubContext) 
            : base(hubContext)
        {
            _personService = personService;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_personService.GetAllPersons());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            return Ok(_personService.Find(id));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Person person)
        {
            _personService.AddPerson(person);
            HubContext.Clients.All.SendAsync("Add", person);
        }

        // PUT api/values/5
        [HttpPut]
        public void Put([FromBody] Person person)
        {
            _personService.UpdatePerson(person);
            HubContext.Clients.All.SendAsync("Change", person);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            _personService.DeletePerson(id);
            HubContext.Clients.All.SendAsync("Delete", id);
        }
    }
}
