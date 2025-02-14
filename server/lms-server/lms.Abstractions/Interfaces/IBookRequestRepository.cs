﻿using lms.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Interfaces
{
    public interface IBookRequestRepository
    {
        Task<List<BookRequest>> GetAllBookRequestsAsync();
        Task<BookRequest> AddNewRequest(BookRequest bookRequest);
        Task<List<BookRequest>> GetBookRequestsByClientId(Guid clientId);
        Task<BookRequest> ApproveRequest(Guid clientId, Guid bookId);
        Task<BookRequest> CancelRequest(Guid clientId, Guid bookId);
        Task<BookRequest> ReturnRequest(Guid clientId, Guid bookId);
        Task<BookRequest> OverdueRequest(Guid clientId, Guid bookId);
    }
}