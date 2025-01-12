using System.Net;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Repositories;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("api/")]
public class UrlShortenerController : ControllerBase
{
    private readonly UrlShortenerRepository _repository;

    public UrlShortenerController(UrlShortenerRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("shorten")]
    public async Task<ObjectResult> SaveUrl([FromBody] SaveUrlRequest saveUrlRequest)
    {
        // Пояснить в README.md почему была выбрана данная база данных
        // Реализовать репозиторий сокращателя ссылок
        // Обработать корректно все виды ошибок в контроллере
        // Рефакторинг

        try
        {
            if (!Uri.TryCreate(saveUrlRequest.Url, UriKind.Absolute, out _))
            {
                return BadRequest("The specified URL is invalid.");
            }

            var code = await _repository.GenerateUniqueCode();

            var request = HttpContext.Request;

            var url = new Url
            {
                LongUrl = saveUrlRequest.Url,
                ShortUrl = $"{request.Scheme}://{request.Host}/{code}",
                Code = code,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _repository.SaveUrl(url);

            return Ok(url.ShortUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode((int)HttpStatusCode.InternalServerError, "");
        }
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetUrl(string code)
    {
        try
        {
            var url = await _repository.GetUrl(code);

            if (url == null)
            {
                return NotFound();
            }

            return Redirect(url.LongUrl);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode((int)HttpStatusCode.InternalServerError, "");
        }
    }
}