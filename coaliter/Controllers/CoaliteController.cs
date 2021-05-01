
using System.IO;
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
  [Route("[controller]/[action]")]
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

    [HttpGet("/Coalite")]
    public ActionResult<CoaliteResource> Coalite()
    {
      var coalite = _coaliter.Get();
      return Ok(coalite);
    }

    [HttpPost]
    public ActionResult<CoaliteResource> Action([FromBody] CoaliteActionRequest coaliteActionRequest)
    {
      var coalite = _coaliter.Action(coaliteActionRequest);
      return Ok(coalite);
    }

    [HttpGet]
    public ActionResult<int> Count()
    {
      var coalite = _coaliter.Count();
      return Ok(coalite);
    }
  }
}