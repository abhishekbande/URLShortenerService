using FluentValidation;
using System.Text.RegularExpressions;
using UrlShortener.Models;

namespace UrlShortner.Api.Validators
{
    public class UrlRequestValidator : AbstractValidator<ShortenUrlRequest>
    {
        public UrlRequestValidator()
        {
            RuleFor(x => x.OriginalUrl)
                .NotEmpty().WithMessage("Original URL cannot be empty.")
                .Must(BeAValidUrl).WithMessage("Invalid URL format.");
        }

        private bool BeAValidUrl(string url)
        {
            // Regex to validate URL format (http or https)
            string pattern = @"^(https?):\/\/([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(:\d+)?(\/[a-zA-Z0-9\-\._~:/?#[\]@!$&'()*+,;%=]*)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(url);
        }
    }
}
