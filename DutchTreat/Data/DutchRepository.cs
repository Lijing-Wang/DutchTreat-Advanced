﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DutchTreatAdvanced.Data
{
    // Another layer between DbContext and its operations
    // Expose the different calls to the database that we want
    // We don't want to use the context directly, we want to make the call through the repository
    public class DutchRepository : IDutchRepository
    {
        private readonly Dutchcontext _context;
        private readonly ILogger<DutchRepository> _logger;

        // ILogger - take it as a generic argument itself
        // Logger will be tied to this type so when it emits data, we will be able to see where the logging came from 
        public DutchRepository(Dutchcontext context, ILogger<DutchRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                _logger.LogInformation("GetProducts is called.");
                return _context.Products
                    .OrderBy(x => x.Title)
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("GetProducts was called", e);
                return null;
            }
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return _context.Products
                .Where(x => x.Category == category)
                .ToList();
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
