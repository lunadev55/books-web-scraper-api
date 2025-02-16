using BooksWebScraperWebAPI.Data;
using BooksWebScraperWebAPI.Interfaces;
using BooksWebScraperWebAPI.Models;
using HtmlAgilityPack;

namespace BooksWebScraperWebAPI.Services
{
    public class ScraperService : IScraperService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(AppDbContext context, ILogger<ScraperService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ScrapeAndStoreBooks()
        {
            _logger.LogInformation("Starting book scraping ({0})...", DateTime.Now);
            var url = "https://books.toscrape.com/catalogue/category/books/travel_2/index.html";
            var links = GetBookLinks(url);
            var books = GetBooks(links);

            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Book scraping completed and saved to database. ({0})", DateTime.Now);
        }

        public List<string> GetBookLinks(string url)
        {
            var doc = GetDocument(url);
            var linkNodes = doc.DocumentNode.SelectNodes("//h3/a");
            var baseUri = new Uri(url);
            return linkNodes.Select(node => new Uri(baseUri, node.Attributes["href"].Value).AbsoluteUri).ToList();
        }

        public List<Book> GetBooks(List<string> links)
        {
            var books = new List<Book>();
            foreach (var link in links)
            {
                var doc = GetDocument(link);
                var book = new Book
                {
                    Title = doc.DocumentNode.SelectSingleNode("//h1").InnerText,
                    Price = ExtractPrice(doc.DocumentNode.SelectSingleNode("//*[@class='col-sm-6 product_main']//*[@class='price_color']").InnerText)
                };
                books.Add(book);
            }
            return books;
        }

        public double ExtractPrice(string raw)
        {
            var reg = new System.Text.RegularExpressions.Regex(@"[\d\.,]+", System.Text.RegularExpressions.RegexOptions.Compiled);
            var match = reg.Match(raw);
            return match.Success ? double.Parse(match.Value) : 0;
        }

        public HtmlDocument GetDocument(string url)
        {
            var web = new HtmlWeb();
            return web.Load(url);
        }
    }
}
