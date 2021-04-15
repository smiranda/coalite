using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ketchup.Pizza.Controllers
{
  [ApiController]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class ErrorController : ControllerBase
  {
    private ILogger _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
      _logger = logger;
    }
    [HttpGet]
    [HttpPost]
    [HttpDelete]
    [HttpPatch]
    [HttpPut]
    [Route("/error")]
    public IActionResult Error()
    {
      var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
      var statusCode = context.Error.Data.Contains("StatusCode") ? context.Error.Data["StatusCode"] : 500;
      var exceptionTitle = context.Error.GetBaseException().GetType().Name;
      // TODO: Could we log everything about the exception here instead of referring to the "above exception" ?
      _logger.LogError($"traceId: {Activity.Current?.Id}, exceptionTitle: {exceptionTitle}, context: <see above exception>");
      return Problem(title: exceptionTitle,
                     statusCode: (int)statusCode);
    }
  }
}