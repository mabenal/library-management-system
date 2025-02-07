using AutoMapper;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookRequestController : ControllerBase
    {
        private readonly IBookRequestRepository bookRequestRepository;
        private readonly IBooksRepository booksRepository;
        private readonly IClientRepository clientRepository;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public BookRequestController(IBookRequestRepository bookRequestRepository, IBooksRepository booksRepository,
            IClientRepository clientRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            this.bookRequestRepository = bookRequestRepository;
            this.booksRepository = booksRepository;
            this.clientRepository = clientRepository;
            this.mapper = mapper;
            this.userManager = userManager;
            this.userService = userService;
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

        [Authorize (Roles = "client")]
        [HttpPost("AddNewRequest")]
        public async Task<ActionResult<BookRequestDto>> AddNewRequest([FromBody] BookRequestDto bookRequestDto)
        {
            try
            {
                var clientId = await userService.GetUserIdAsync(User);
                if (clientId == null)
                {
                    return Unauthorized();
                }

                var bookRequestDomainModel = mapper.Map<BookRequest>(bookRequestDto);
                bookRequestDomainModel.ClientId = clientId.Value;

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
            }
        }

        [Authorize (Roles = "client")]
        [HttpGet("GetBookRequestsByClient")]
        public async Task<ActionResult<BookRequestDto>> GetBookRequestsByClient()
        {
            try
            {
                var clientId = await userService.GetUserIdAsync(User);
                if (clientId == null)
                {
                    return Unauthorized();
                }

                var bookRequests = await bookRequestRepository.GetBookRequestsByClientId(clientId.Value);
                return Ok(mapper.Map<List<BookRequestDto>>(bookRequests));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
            }
        }

        [Authorize (Roles = "client")]
        [HttpPut("CancelRequest/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> CancelRequest([FromRoute] Guid bookId)
        {
            try
            {
                var clientId = await userService.GetUserIdAsync(User);
                if (clientId == null)
                {
                    return Unauthorized();
                }

                var bookRequest = await bookRequestRepository.CancelRequest(clientId.Value, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
            }
        }

        [Authorize (Roles = "librarian")]
        [HttpPut("CancelRequestByClient/{clientId:Guid}/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> CancelRequestByClient([FromRoute] Guid bookId, [FromRoute] Guid clientId)
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
            }
        }

        [Authorize (Roles = "librarian")]
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

        [Authorize (Roles = "client")]
        [HttpPut("ReturnRequest/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> ReturnRequest([FromRoute] Guid bookId)
        {
            try
            {
                var clientId = await userService.GetUserIdAsync(User);
                if (clientId == null)
                {
                    return Unauthorized();
                }

                var bookRequest = await bookRequestRepository.ReturnRequest(clientId.Value, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
            }
        }

        [HttpPut("OverdueRequest/{bookId:Guid}")]
        public async Task<ActionResult<BookRequestDto>> OverdueRequest([FromRoute] Guid bookId)
        {
            try
            {
                var clientId = await userService.GetUserIdAsync(User);
                if (clientId == null)
                {
                    return Unauthorized();
                }

                var bookRequest = await bookRequestRepository.OverdueRequest(clientId.Value, bookId);
                return Ok(mapper.Map<BookRequestDto>(bookRequest));
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in BookRequestController: {e}");
            }
        }
    }
}