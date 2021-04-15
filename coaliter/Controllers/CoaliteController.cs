
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ketchup.Pizza.DB;
using Ketchup.Pizza.Models;
using Ketchup.Pizza.Services;

namespace Ketchup.Pizza.Controllers
{
  [ApiController]
  [Route("coalite")]
  public class CoaliteController : ControllerBase
  {
    private readonly ILogger<CoaliteController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICoaliter _coaliter;
    public CoaliteController(IConfiguration configuration,
                             ILogger<CoaliteController> logger,
                             ICoaliter coaliter)
    {
      _logger = logger;
      _configuration = configuration;
      _coaliter = coaliter;
    }

    [HttpGet("")]
    public ActionResult<CoaliteResource> Get()
    {
      var claimId = "System"; // Could be negotiated with the outside.
      var coalite = _coaliter.Get(claimId);
      return Ok(coalite);
    }

    [HttpGet("/count")]
    public ActionResult<int> GetCount()
    {
      var coalite = _coaliter.Count();
      return Ok(coalite);
    }
  }
}