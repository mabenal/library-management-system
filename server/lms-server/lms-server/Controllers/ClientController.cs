using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms_server.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace lms_server.Controllers
{
    public class ClientController : Controller
    {
        private readonly IBooksRepository booksRepository;
        private IMapper mapper { get; }

        public ClientController(IBooksRepository booksRepository, IMapper mapper)
        {
            this.booksRepository = booksRepository;
            this.mapper = mapper;
        }

        [HttpGet("GetAllClients")]
        public async Task<ActionResult<Client>> GetAllClients()
        {
            try
            {
                var clients = await booksRepository.GellAllClientsAsync();
                return Ok(mapper.Map<List<ClientDto>>(clients));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in clientsController: {e}");
                throw;
            }
        }

        [HttpGet("GetClient/{id:Guid}")]
        public async Task<ActionResult<ClientDto>> GetClientByID([FromRoute] Guid id)
        {
            try
            {
                var client = await booksRepository.GetClientByID(id);
                return Ok(mapper.Map<ClientDto>(client));
            }
            catch (Exception e)
            {
                Console.WriteLine($"in clientsController:  {e}");
                throw;
            }

        }

        [HttpPut("UpdateClient/{id:Guid}")]
        public async Task<ActionResult<ClientDto>> UpdateClientDetails([FromRoute] Guid id, [FromBody] ClientDto client)
        {
            try
            {
                var clientdetails = mapper.Map<Client>(client);

                clientdetails = await booksRepository.UpdateClientDetails(id, clientdetails);

                if(clientdetails == null)
                {
                    return NotFound();
                }
                else
                {
                    var clientDto = mapper.Map<ClientDto>(clientdetails);

                    return Ok(clientDto);
                }
 
            }
            catch (Exception e)
            {
                Console.WriteLine($"in clientsController:  {e}");
                throw;
            }
        }

        [HttpDelete("DeleteClient/{id:Guid}")]
        public async Task<ActionResult<ClientDto>> DeleteClient([FromRoute] Guid id)
        {
            try
            {
                var client = await booksRepository.DeleteClientAsync(id);

                if(client == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(mapper.Map<ClientDto>(client));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"in clientsController:  {e}");
                throw;
            }
        }
    }
}
