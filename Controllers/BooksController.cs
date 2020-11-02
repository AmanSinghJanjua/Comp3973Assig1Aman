using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using ConsumeGoogleAPI.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;
using ConsumeGoogleApi.Models;
using System.Diagnostics;

namespace ConsumeGoogleAPI.Controllers
{
    public class BooksController : Controller
    {
        const string BASE_URL = "https://www.googleapis.com/books/v1/";

        private readonly ILogger<BooksController> _logger;

        private readonly IHttpClientFactory _clientFactory;

        public IEnumerable<Book> Books { get; set; }

        public bool GetStudentsError { get; private set; }

        public BooksController(ILogger<BooksController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        private async Task<IEnumerable<Book>> GetBooks()
        {

            IList<Book> Books = new List<Book>();

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"{BASE_URL}volumes?q=harry+potter");
            message.Headers.Add("Accept", "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(message);


            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                string JsonString = StreamToString(responseStream);
                dynamic myJObject = JObject.Parse(JsonString);
                dynamic Tuples = myJObject.items;
                for (int i = 0; i < Tuples.Count; i++)
                {
                    string id = Tuples[i].id;
                    string title = Tuples[i].volumeInfo.title;
                    string smallImageLink = Tuples[i].volumeInfo.imageLinks.smallThumbnail;
                    dynamic listAuthors = Tuples[i].volumeInfo.authors;
                    List<string> authors = new List<string>();
                    for (int j = 0; j < listAuthors.Count; j++)
                    {
                        string authorName = listAuthors[j];
                        authors.Add(authorName);
                    }
                    string publisher = Tuples[i].volumeInfo.publisher;
                    string publishedDate = Tuples[i].volumeInfo.publishedDate;
                    string description = Tuples[i].volumeInfo.description;
                    string iSBN10 = Tuples[i].volumeInfo.industryIdentifiers[1].identifier;
                    Book book = new Book { Id = id, Title = title, SmallImageLink = smallImageLink, Authors = authors, Publisher = publisher, PublishedDate = publishedDate, Description = description, ISBN10 = iSBN10 };
                    Books.Add(book);
                }
            }
            else
            {
                GetStudentsError = true;
                Books = Array.Empty<Book>();
            }
            return Books;
        }
        public string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<IActionResult> Index()
        {
            Books = await GetBooks();

            return View(Books);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = new HttpRequestMessage();
            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"{BASE_URL}volumes/" + id);
            message.Headers.Add("Accept", "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(message);

            Book book = null;

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                string JsonString = StreamToString(responseStream);
                dynamic Tuples = JObject.Parse(JsonString);
                string title = Tuples.volumeInfo.title;
                string smallImageLink = Tuples.volumeInfo.imageLinks.smallThumbnail;
                dynamic listAuthors = Tuples.volumeInfo.authors;
                List<string> authors = new List<string>();
                for (int j = 0; j < listAuthors.Count; j++)
                {
                    string authorName = listAuthors[j];
                    authors.Add(authorName);
                }
                string publisher = Tuples.volumeInfo.publisher;
                string publishedDate = Tuples.volumeInfo.publishedDate;
                string description = Tuples.volumeInfo.description;
                string iSBN10 = Tuples.volumeInfo.industryIdentifiers[1].identifier;
                book = new Book { Id = id, Title = title, SmallImageLink = smallImageLink, Authors = authors, Publisher = publisher, PublishedDate = publishedDate, Description = description, ISBN10 = iSBN10 };
            }
            else
            {
                GetStudentsError = true;
            }

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
