using Microsoft.Extensions.Options;
using Shorty.Configs;
using Shorty.Exceptions;
using Shorty.Models;

namespace Shorty.Services
{
    public class UrlService : IUrlService
    {
        private readonly EncoderConfig config;
        private readonly IDataService dataService;
        private readonly IEncodingService encodingService;

        public UrlService(IOptions<EncoderConfig> config, IDataService dataService, IEncodingService encodingService)
        {
            this.config = config.Value;
            this.dataService = dataService;
            this.encodingService = encodingService;
        }

        public async Task<string> ExpandUrl(string shortUrl)
        {
            try
            {
                var uri = new Uri(shortUrl);
                var urlTag = uri.PathAndQuery.TrimStart('/');

                // NOTE: in a real world app we would likely increment a hit counter, record some
                //       metadata, or take some kind of action when a url is decoded.
                var urlRecord = await dataService.FindUrlDocumentByTag(urlTag);

                if (urlRecord == null) throw new UnknownUrlException();

                return urlRecord.FullUrl;
            }
            catch(UriFormatException)
            {
                throw new InvalidUrlException();
            }
        }

        public async Task<string> ShortenUrl(string fullUrl)
        {
            try
            {
                // NOTE: this assumes a well formed web URL that includes the protocol.
                //       If this is not a valid assumption, then the incoming URL will
                //       need to be coerced into an acceptable format.
                //       Example: 'www.mycompany.com' --> 'https://www.mycompany.com'
                var uri = new Uri(fullUrl);
                var urlRecord = (await dataService.FindUrl(uri)) ?? (await CreateUrl(uri));

                return BuildShortUrl(urlRecord.Tag);
            }
            catch(UriFormatException)
            {
                throw new InvalidUrlException();
            }
        }

        protected string BuildShortUrl(string urlTag)
        {
            var baseUri = new Uri(config.BaseUrl);
            return new Uri(baseUri, urlTag).ToString();
        }

        protected async Task<UrlDocument> CreateUrl(Uri uri)
        {
            var url = new UrlDocument()
            {
                FullUrl = uri.ToString(),
                Tag = encodingService.CreateUrlTag()
            };

            await dataService.SaveUrl(url);
            return url;
        }
    }
}
