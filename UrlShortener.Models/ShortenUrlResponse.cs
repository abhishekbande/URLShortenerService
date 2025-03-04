using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Models
{
    public class ShortenUrlResponse
    {
        public string ShortId { get; set; }

        public string ShortUrl { get; set; }
    }
}
