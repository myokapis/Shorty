using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shorty.Exceptions;

namespace Shorty.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index([FromServices] IHostEnvironment hostEnvironment)
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            // NOTE: this pattern of only broadcasting safe error messages is a response to
            //       having experienced third party code that spewed too much information in
            //       error messages. Saw one case where sensitive data was leaked.
            var isSafeError = feature.Error.GetType().IsSubclassOf(typeof(ShortyException));
            var message = isSafeError ? feature.Error.Message : "Unknown Error";

            // TODO: sanitize the raw error to mask any sensitive data if logging to an
            //       insecure destination.
            // TODO: augment the log with Trace Id
            Log.Error(feature.Error, "Unknown Error");

            return Problem(message);
        }
    }
}
