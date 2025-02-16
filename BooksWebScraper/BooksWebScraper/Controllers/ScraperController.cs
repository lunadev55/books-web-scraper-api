using BooksWebScraperWebAPI.Data;
using BooksWebScraperWebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksWebScraperWebAPI.Controllers
{
    [ApiController]
    [Route("api/scraper")]
    public class ScraperController : ControllerBase
    {        
        private readonly IScraperService _scraperService;
        private readonly AppDbContext _context;

        public ScraperController(IScraperService scraperService, AppDbContext context)
        {
            _scraperService = scraperService;
            _context = context;
        }

        [HttpPost("scrape")] // Trigger scraping on demand
        public async Task<IActionResult> ScrapeBooks()
        {
            await _scraperService.ScrapeAndStoreBooks();
            return Ok("Scraping started successfully.");
        }

        [HttpGet("books")] // Retrieve all scraped books
        public async Task<IActionResult> GetScrapedBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }
    }
}