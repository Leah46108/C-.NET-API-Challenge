using CsvHelper.Configuration;
using CsvHelper;
using PizzaSales.Data;
using System.Globalization;
using PizzaSales.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PizzaSales.Services
{
    public class CsvService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CsvService> _logger;

        public CsvService(AppDbContext context, ILogger<CsvService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Sales> ImportSales(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    //BadDataFound = context => throw new CsvHelperException($"Bad data found on row '{context.Raw}'")
                }))
                {
                    var records = csv.GetRecords<Sales>().ToList();

                    // Validate data before importing
                    var validationErrors = ValidateCsvData(records);
                    if (validationErrors.Any())
                    {
                        throw new Exception($"CSV data validation failed: {string.Join(", ", validationErrors)}");
                    }

                    _context.Sales.AddRange(records);
                    _context.SaveChanges();

                    return records;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing CSV data");
                throw;
            }
        }

        private List<string> ValidateCsvData(List<Sales> records)
        {
            var errors = new List<string>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Name))
                {
                    errors.Add("Name cannot be empty");
                }

                if (record.Price <= 0)
                {
                    errors.Add("Price must be greater than zero");
                }

                if (record.Quantity <= 0)
                {
                    errors.Add("Quantity must be greater than zero");
                }
            }

            return errors;
        }
    }
}
