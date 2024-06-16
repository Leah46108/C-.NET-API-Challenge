using Microsoft.AspNetCore.Mvc;
using PizzaSales.Data;
using PizzaSales.Services;
using PizzaSales.Models;

namespace PizzaSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CsvService _csvService;

        public SalesController(AppDbContext context, CsvService csvService)
        {
            _context = context;
            _csvService = csvService;
        }

        // GET: api/sales
        [HttpGet]
        public ActionResult<IEnumerable<Sales>> GetSales()
        {
            return _context.Sales.ToList();
        }

        // GET: api/sales/{id}
        [HttpGet("{id}")]
        public ActionResult<Sales> GetSalesById(int id)
        {
            var sales = _context.Sales.Find(id);

            if (sales == null)
            {
                return NotFound();
            }

            return sales;
        }

        // POST: api/sales/import
        [HttpPost("import")]
        public IActionResult ImportCsv([FromForm] FileUploadModel fileModel)
        {
            if (fileModel == null || fileModel.File == null || fileModel.File.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                var filePath = Path.GetTempFileName();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileModel.File.CopyTo(stream);
                }

                var importedData = _csvService.ImportSales(filePath);

                return Ok(importedData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
