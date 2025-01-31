using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lms.Abstractions.Interfaces;
using AutoMapper;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Models;
using lms.Abstractions.Exceptions;

namespace lms_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                var clients = await clientsRepository.GetAllClientsAsync();
                return Ok(mapper.Map<List<ClientDto>>(clients));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in clientsController: {e}");
                throw;
            }
        }

        [HttpGet("GetClient/{id:Guid}")]
        public async Task<ActionResult<ClientDto>> GetClientByID([FromRoute] Guid id)
        {
            try
            {
                var client = await clientsRepository.GetClientById(id);
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
                var updatedClient = await clientsRepository.UpdateClientDetails(id, mapper.Map<Client>(client));
                return Ok(mapper.Map<ClientDto>(updatedClient));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in clientsController: {e}");
                throw;
            }
        }

        [HttpDelete("DeleteClient/{id:Guid}")]
        public async Task<ActionResult<ClientDto>> DeleteClient([FromRoute] Guid id)
        {
            try
            {
                var deletedClient = await clientsRepository.DeleteClientAsync(id);
                return Ok(mapper.Map<ClientDto>(deletedClient));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in clientsController: {e}");
                throw;
            }
        }
    }
}