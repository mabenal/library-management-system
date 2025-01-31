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

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookRequestController : ControllerBase
    {
        public readonly IBookRequestRepository bookRequestRepository;
        public readonly IBooksRepository booksRepository;
        public readonly IClientRepository clientRepository;
        private IMapper mapper { get; }

        public BookRequestController(IBookRequestRepository bookRequestRepository, IBooksRepository booksRepository, IClientRepository clientRepository, IMapper mapper)
        {
            this.bookRequestRepository = bookRequestRepository;
            this.booksRepository = booksRepository;
            this.clientRepository = clientRepository;
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
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPost("AddNewRequest")]
        public async Task<ActionResult<BookRequestDto>> AddNewRequest([FromBody] BookRequestDto bookRequestDto)
        {
            try
            {
                var bookRequestDomainModel = mapper.Map<BookRequest>(bookRequestDto);
                var bookRequest = await bookRequestRepository.AddNewRequest(bookRequestDomainModel);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpGet("GetBookRequestsByClientId/{clientId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> GetBookRequestsByClientId([FromRoute] Guid clientId)
        {
            try
            {
                var bookRequests = await bookRequestRepository.GetBookRequestsByClientId(clientId);
                return Ok(mapper.Map<List<BookRequestDto>>(bookRequests));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPut("CancelRequest/{clientId:Guid}/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> CancelRequest([FromRoute] Guid clientId, [FromRoute] Guid bookId)
        {
            try
            {
                var bookRequest = await bookRequestRepository.CancelRequest(clientId, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPut("ApproveRequest/{clientId:Guid}/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> ApproveRequest([FromRoute] Guid clientId, [FromRoute] Guid bookId)
        {
            try
            {
                var bookRequest = await bookRequestRepository.ApproveRequest(clientId, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPut("ReturnRequest/{clientId:Guid}/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> ReturnRequest([FromRoute] Guid clientId, [FromRoute] Guid bookId)
        {
            try
            {
                var bookRequest = await bookRequestRepository.ReturnRequest(clientId, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }

        [HttpPut("OverdueRequest/{clientId:Guid}/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> OverdueRequest([FromRoute] Guid clientId, [FromRoute] Guid bookId)
        {
            try
            {
                var bookRequest = await bookRequestRepository.OverdueRequest(clientId, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
                throw;
            }
        }
    }
}