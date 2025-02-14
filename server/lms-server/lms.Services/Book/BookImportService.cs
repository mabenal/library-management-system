﻿using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace lms.Services
{
    public class BookImportService
    {
        private readonly ILmsDbContext _dbContext;

        public BookImportService(ILmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ImportBooksAsync(string filePath)
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            var books = JsonConvert.DeserializeObject<List<Book>>(jsonData);

            if (!_dbContext.Books.Any())
            {
                foreach (var book in books)
                {
                    _dbContext.Books.Add(book);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}