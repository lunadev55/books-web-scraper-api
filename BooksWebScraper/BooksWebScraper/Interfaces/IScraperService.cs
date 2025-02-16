using BooksWebScraperWebAPI.Models;
using HtmlAgilityPack;

namespace BooksWebScraperWebAPI.Interfaces
{
    public interface IScraperService
    {
        Task ScrapeAndStoreBooks();
        List<string> GetBookLinks(string url);
        List<Book> GetBooks(List<string> links);
        double ExtractPrice(string raw);
        HtmlDocument GetDocument(string url);
    }
}
