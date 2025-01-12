using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Repositories.Base;
using UrlShortener.Web.Repositories.Constants;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Repositories;

public class UrlShortenerRepository
{
    private readonly UrlShortenerDbContext _context;
    private readonly Random _random = new();
    private readonly int _maxNumberOfAttempts = 100;

    public UrlShortenerRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[ShortLinkSettings.Length];
        var maxValue = ShortLinkSettings.Alphabet.Length;
        var numberOfAttempts = 0;

        while (numberOfAttempts < _maxNumberOfAttempts)
        {
            for (var i = 0; i < ShortLinkSettings.Length; i++)
            {
                var randomIndex = _random.Next(maxValue);

                codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
            }

            var code = new string(codeChars);

            if (!await _context.Urls.AnyAsync(u => u.Code == code))
            {
                return code;
            }
        }

        throw new Exception("The number of attempts to create a unique code has expired.");
    }

    public async Task SaveUrl(Url url)
    {
        _context.Urls.Add(url);
        await _context.SaveChangesAsync();
    }

    public async Task<Url?> GetUrl(string code)
    {
        var url = await _context.Urls.SingleOrDefaultAsync(u => u.Code == code);

        return url;
    }
}