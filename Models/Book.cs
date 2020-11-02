using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumeGoogleAPI.Models
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SmallImageLink { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public string ISBN10 { get; set; }
    }
}
