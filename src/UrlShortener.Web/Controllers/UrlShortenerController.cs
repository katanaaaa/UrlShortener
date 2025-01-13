using System.Net;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Repositories;
using UrlShortener.Web.Repositories.Constants;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("")]
public class UrlShortenerController(UrlShortenerRepository repository) : ControllerBase
{
    private const int MaxUrlLength = 2048;
    
    [HttpPost("shorten")]
    public async Task<IActionResult> SaveUrl([FromBody] SaveUrlRequest saveUrlRequest)
    {
        try
        {
            if (!Uri.TryCreate(saveUrlRequest.Url, UriKind.Absolute, out _))
            {
                return BadRequest("URL is invalid.");
            }

            if (saveUrlRequest.Url.Length > MaxUrlLength)
            {
                return BadRequest("URL length increased");
            }
            
            var code = await repository.GenerateUniqueCode();

            var request = HttpContext.Request;

            var url = new Url
            {
                LongUrl = saveUrlRequest.Url,
                ShortUrl = $"{request.Scheme}://{request.Host}/{code}",
                Code = code,
                CreatedOnUtc = DateTime.UtcNow
            };

            await repository.SaveUrl(url);

            return Ok(url.ShortUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode((int)HttpStatusCode.InternalServerError, UrlShortenerConstants.CommonServerErrorText);
        }
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetUrl(string code)
    {
        try
        {
            var url = await repository.GetUrl(code);

            if (url == null)
            {
                return NotFound();
            }

            return Redirect(url.LongUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode((int)HttpStatusCode.InternalServerError, UrlShortenerConstants.CommonServerErrorText);
        }
    }
}