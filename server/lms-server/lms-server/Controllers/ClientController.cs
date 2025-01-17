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
        private readonly IClientRepository clientsRepository;
        private IMapper mapper { get; }

        public ClientController(IClientRepository clientsRepository, IMapper mapper)
        {
            this.clientsRepository = clientsRepository;
            this.mapper = mapper;
        }

        [HttpGet("GetAllClients")]
        public async Task<ActionResult<ClientDto>> GetAllClients()
        {
            try
            {
                var clients = await clientsRepository.GellAllClientsAsync();
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
                var client = await clientsRepository.GetClientByID(id);
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

                clientdetails = await clientsRepository.UpdateClientDetails(id, clientdetails);

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
                var client = await clientsRepository.DeleteClientAsync(id);

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
