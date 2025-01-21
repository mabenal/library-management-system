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

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BookRequestController : ControllerBase
    {
        public readonly IBookRequestRepository bookRequestRepository;
        private  IMapper mapper { get; }

        public BookRequestController(IBookRequestRepository bookRequestRepository, IMapper mapper)
        {
            this.bookRequestRepository = bookRequestRepository;
            this.mapper = mapper;
        }

        [HttpGet("GetAllBookRequests")]
        public async Task<ActionResult<BookRequestDto>> GetAllBookRequests()
        {
            try
            {
                var bookRequests = await bookRequestRepository.GetAllBookRequestsAsync();
                return Ok(mapper.Map<List<BookRequestDto>>(bookRequests));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPost("AddNewRequest")]
        public async Task<ActionResult<BookRequestDto>> AddNewRequest([FromBody]BookRequestDto bookRequestDto)
        {
            try
            {

                var bookRequestDomainModel = mapper.Map<BookRequest>(bookRequestDto);
                var bookRequest = await bookRequestRepository.AddNewRequest(bookRequestDomainModel);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in BookRequestController: {e}");
                throw;
            }
        }

    }
}
