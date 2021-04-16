
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
  public class KeyController : ControllerBase
  {
    private readonly ILogger<KeyController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ICoaliter _coaliter;
    private string _publicKey;

    public KeyController(IConfiguration configuration,
                         ILogger<KeyController> logger,
                         ICoaliter coaliter)
    {
      _logger = logger;
      _configuration = configuration;
      _coaliter = coaliter;

      var publicKeyPath = _configuration["PublicKey"];
      _publicKey = System.IO.File.ReadAllText(publicKeyPath);
    }

    [HttpGet("/Key")]
    public ActionResult<string> Key()
    {
      return Ok(_publicKey);
    }
  }
}