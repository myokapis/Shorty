using Microsoft.AspNetCore.Mvc;
using Shorty.Services;

namespace Shorty.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController(IUrlService urlService) : Controller
    {
        private readonly IUrlService urlService = urlService;

        [HttpPost("ExpandUrl", Name = "ExpandUrl")]
        public async Task<IActionResult> ExpandUrl([FromBody] string shortUrl)
        {
            return Ok(await urlService.ExpandUrl(shortUrl));
        }

        [HttpPost("ShortenUrl", Name = "ShortenUrl")]
        public async Task<IActionResult> ShortenUrl([FromBody] string fullUrl)
        {
            return Ok(await urlService.ShortenUrl(fullUrl));
        }
    }
}
