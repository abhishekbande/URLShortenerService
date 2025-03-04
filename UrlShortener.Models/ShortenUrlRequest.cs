using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class ShortenUrlRequest
    {
        [Required(ErrorMessage = "OriginalUrl is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string OriginalUrl { get; set; }
    }
}
