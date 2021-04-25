using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ketchup.Pizza.DB;
using Ketchup.Pizza.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Ketchup.Pizza.Services
{
  public class Coaliter : BackgroundService, ICoaliter
  {
    private object _dblock;
    private ILogger _logger;
    private string _keypath;
    private string _passphrase;
    private RSACryptoServiceProvider _rsaProvider;
    private string _connectionString;
    private static DateTime _baseDate = new DateTime(year: 2021, month: 4, day: 15, hour: 22, minute: 23, second: 0);
    private DbContextOptionsBuilder<CoaliteDBContext> _connectionOptionsBuilder;

    public Coaliter(IConfiguration configuration,
                    ILogger<Coaliter> logger)
    {
      _keypath = configuration["PrivateKey"];
      _passphrase = configuration["Keypassphrase"];
      var keyData = File.ReadAllText(_keypath);

      _rsaProvider = new RSACryptoServiceProvider();
      _rsaProvider.ImportFromPem(new ReadOnlySpan<char>(keyData.ToCharArray()));

      _connectionString = configuration["State:ConnectionString"];
      _connectionOptionsBuilder = new DbContextOptionsBuilder<CoaliteDBContext>();
      _connectionOptionsBuilder.UseSqlite(_connectionString);
      _dblock = new object();
      _logger = logger;
      var dbcontext = GetDBConnection();
      dbcontext.Database.EnsureCreated(); // simple db = simple approach.
    }

    public CountResource Count()
    {
      lock (_dblock)
      {
        var dbcontext = GetDBConnection();
        var count = dbcontext.Coalites.Count();
        return new CountResource(count);
      }
    }
    public CoaliteResource Get()
    {
      Coalite coalite;
      lock (_dblock)
      {
        var dbcontext = GetDBConnection();
        coalite =
          dbcontext.Coalites
                    .OrderByDescending(c => c.FullSecondStamp)
                    .FirstOrDefault();
        if (coalite.Claimed)
        {
          throw new CoalitingException((int)HttpStatusCode.BadRequest, "Current coalite already emitted");
        }
      }
      var coaliteTs = _baseDate + TimeSpan.FromSeconds(coalite.FullSecondStamp);

      var dataToSign = "";// TODO: Use new extension method
      var signature = Convert.ToBase64String(_rsaProvider
                                             .SignData(Encoding.UTF8.GetBytes(dataToSign),
                                                       SHA256.Create()));
      var signatures = new List<CoaliteSignature>();
      signatures.Add(new CoaliteSignature(signature, CoaliteAction.EMIT, "", "System"));
      return new CoaliteResource(coalid: coalite.Coalid,
                                 payload: coalite.Payload,
                                 signatures: signatures,
                                 seqid: coalite.FullSecondStamp,
                                 timestamp: coaliteTs);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      do
      {
        if (stoppingToken.IsCancellationRequested)
          break;

        var now = DateTime.UtcNow;
        var diffSeconds = (now - _baseDate).TotalSeconds;
        int fullSecondStamp = (int)Math.Floor(diffSeconds);

        lock (_dblock)
        {
          var dbcontext = GetDBConnection();
          var coalite = dbcontext.Coalites
                                 .Where(c => c.FullSecondStamp == fullSecondStamp)
                                 .FirstOrDefault();
          if (coalite == null)
          {
            var newCoalite = GenerateCoalite(fullSecondStamp);
            dbcontext.Add(newCoalite);
            dbcontext.SaveChanges();
          }
        }

        var nextNow = DateTime.UtcNow;
        var nextDiffSeconds = (nextNow - _baseDate).TotalSeconds;
        int nextFullSecondStamp = (int)Math.Floor(nextDiffSeconds);

        double awaitSeconds = 0.0;
        int fdiff = nextFullSecondStamp - fullSecondStamp;
        if (fdiff - 1 > 0)
        {
          var coalitesLate = fdiff - 1;
          _logger.LogWarning($"{coalitesLate} Coalites Lost ! Hurrying up...");
          continue;
        }
        else if (fdiff == 1)
        {
          // Already in next second, but don't worry, there's still time.
          continue;
        }

        // still in same second
        awaitSeconds = 1.0 - (nextDiffSeconds - nextFullSecondStamp);

        _logger.LogInformation($"Waiting {awaitSeconds}s");
        await Task.Delay(TimeSpan.FromSeconds(awaitSeconds), stoppingToken);
      } while (true);
      _logger.LogWarning("Execution cancelled");
    }

    private Coalite GenerateCoalite(int fullSecondStamp)
    {
      var coalid = Guid.NewGuid().ToString();
      var payload = new CoalitePayload(new List<CoaliteSignature>());
      var serializedPayload = JObject.FromObject(payload).ToString(Newtonsoft.Json.Formatting.None);
      var coalite = new Coalite(coalid, serializedPayload, fullSecondStamp);
      return coalite;
    }


    private CoaliteDBContext GetDBConnection()
    {
      return new CoaliteDBContext(new SqliteDbDefaults(), _connectionOptionsBuilder.Options);
    }
    public CoaliteResource Action(CoaliteActionRequest coaliteActionRequest)
    {
      Coalite coalite;
      lock (_dblock)
      {
        var dbcontext = GetDBConnection();
        coalite =
          dbcontext.Coalites
                   .Where(c => c.Coalid == coaliteActionRequest.Coalid)
                   .FirstOrDefault();
        if (coalite == null)
        {
          throw new CoalitingException((int)HttpStatusCode.BadRequest, "Unexisting coalite");
        }

        var payload = JToken.Parse(coalite.Payload).ToObject<CoalitePayload>();

        // Find coalite owner
        var lastClaimAction = payload.Signatures.LastOrDefault(s => s.Action == CoaliteAction.CLAIM);

        // Case 1: No owner and action is to claim
        if (lastClaimAction == null && coaliteActionRequest.Signature.Action == CoaliteAction.CLAIM)
        {
          throw new NotImplementedException();
        }

        // Case 2
        // Check if claimer has ownership rights
        var pubkey = coaliteActionRequest.SignerPublicKey;

        //TODO _rsaProvider.VerifyData();

      }

      throw new NotImplementedException();
    }
  }
}