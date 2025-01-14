namespace Shorty.Services
{
    public interface IUrlService
    {
        Task<string> ExpandUrl(string shortUrl);
        Task<string> ShortenUrl(string fullUrl);
    }
}
