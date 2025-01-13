using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Repositories.Base;
using UrlShortener.Web.Repositories.Constants;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Repositories;

public class UrlShortenerRepository(UrlShortenerDbContext context)
{
    private readonly Random _random = new();
    private const int MaxNumOfAttempts = 100;

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[ShortLinkSettings.Length];
        var maxValue = ShortLinkSettings.Alphabet.Length;
        var numOfAttempts = 0;

        while (numOfAttempts < MaxNumOfAttempts)
        {
            for (var i = 0; i < ShortLinkSettings.Length; i++)
            {
                var randomIndex = _random.Next(maxValue);
                codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
            }

            var code = new string(codeChars);

            if (!await context.Urls.AnyAsync(u => u.Code == code))
            {
                return code;
            }

            numOfAttempts++;
        }

        throw new Exception("The number of attempts to create a unique code has expired.");
    }
    
    public async Task<Url?> GetUrl(string code)
        => await context.Urls.SingleOrDefaultAsync(u => u.Code == code);

    public async Task SaveUrl(Url url)
    {
        context.Urls.Add(url);
        await context.SaveChangesAsync();
    }
}