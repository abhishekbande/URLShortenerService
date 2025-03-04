using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services.Interfaces;

namespace UrlShortner.Api.Controllers;

/// <summary>
/// Controller for managing URL shortening and resolving shortened URLs.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly ILogger<UrlController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlController"/> class.
    /// </summary>
    /// <param name="urlShortenerService">The URL shortener service for handling URL shortening and resolving.</param>
    /// <param name="logger">The logger for capturing logs related to URL operations.</param>
    public UrlController(IUrlShortenerService urlShortenerService, ILogger<UrlController> logger)
    {
        _urlShortenerService = urlShortenerService;
        _logger = logger;
    }

    /// <summary>
    /// Shortens the provided original URL.
    /// </summary>
    /// <param name="request">The request containing the original URL to be shortened.</param>
    /// <returns>A response containing the shortened URL or an error message.</returns>
    [HttpPost("shorten")]
    public IActionResult ShortenUrl([FromBody] ShortenUrlRequest request)
    {
        try
        {
            // Validate that the original URL is not empty
            if (string.IsNullOrEmpty(request?.OriginalUrl))
            {
                _logger.LogWarning("Attempt to shorten an empty or null URL.");
                return BadRequest("URL cannot be empty.");
            }

            // Shorten the URL
            var shortUrlResponse = _urlShortenerService.ShortenUrl(request.OriginalUrl);

            // Check if the URL was already shortened
            if (shortUrlResponse == null)
            {
                _logger.LogWarning($"URL could not be shortned or already shortened: {request.OriginalUrl}");
                return Conflict("URL already shortened.");
            }

            // Return the shortened URL
            _logger.LogInformation($"Successfully shortened URL: {request.OriginalUrl} to {shortUrlResponse.ShortUrl}");
            return Ok(shortUrlResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while shortening the URL.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Resolves a shortened URL back to its original URL.
    /// </summary>
    /// <param name="shortId">The short identifier for the shortened URL.</param>
    /// <returns>A response containing the original URL or an error message if not found.</returns>
    [HttpGet("{shortId}")]
    public IActionResult ResolveUrl(string shortId)
    {
        try
        {
            // Resolve the original URL from the short ID
            var originalUrl = _urlShortenerService.ResolveUrl(shortId);

            // If the original URL is not found, return a 404
            if (string.IsNullOrEmpty(originalUrl))
            {
                _logger.LogWarning($"Shortened URL not found for Short ID: {shortId}");
                return NotFound("Shortened URL not found.");
            }

            // Return the original URL
            _logger.LogInformation($"Resolved Short ID: {shortId} to Original URL: {originalUrl}");
            return Ok(new { OriginalUrl = originalUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resolving the shortened URL.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
