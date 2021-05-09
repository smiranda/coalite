using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Pizza.Models;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace coahoarder
{

  // Example to claim and burn
  // dotnet run http://localhost:5000 hoarder CLAIM ../keys/key.pem ../keys/key.pub > coalite.json
  // dotnet run http://localhost:5000 hoarder BURN ../keys/key.pem ../keys/key.pub coalite.json
  class Program
  {
    static async Task Main(string[] args)
    {
      if (args.Length < 2)
      {
        var versionString = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                .InformationalVersion
                                .ToString();

        Console.WriteLine($"coaget v{versionString}");
        Console.WriteLine("-------------");
        Console.WriteLine("\nUsage:");
        Console.WriteLine("  coaget <coalite_server_uri> <user_tag> [action=CLAIM] [priv_key_path=key.pkcs1.prv] [pub_key_path=key.pkcs1.pub] [existing_coalite=None] [mint_payload=payload.b64]");
        return;
      }
      var coaliteServerUri = args[0];
      var userTag = args[1];
      var coaliteAction = args.Length > 2 ? Enum.Parse<CoaliteAction>(args[2]) : CoaliteAction.CLAIM;
      var privateKeyData = args.Length > 3 ? File.ReadAllText(args[3]) : File.ReadAllText("key.prv");
      var publicKeyData = args.Length > 4 ? File.ReadAllText(args[4]) : File.ReadAllText("key.pub").Trim();
      CoaliteResource existingCoalite = args.Length > 5 ? JToken.Parse(File.ReadAllText(args[5])).ToObject<CoaliteResource>() : null;
      var mintPayload = coaliteAction == CoaliteAction.MINT ? (args.Length > 6 ? File.ReadAllText(args[6]) : File.ReadAllText("payload.b64").Trim()) : "";


      RSA rsa = RSA.Create();
      int bytesRead;
      rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyData), out bytesRead);

      var httpClient = new HttpClient();
      CoaliteResource coaliteResource = null;
      if (coaliteAction == CoaliteAction.CLAIM)
      {
        // In this case we need to get a new coalite to claim
        var response = await httpClient.GetStringAsync($"{coaliteServerUri}/coalite");//"http://localhost:5000/coalite"
        coaliteResource = JToken.Parse(response).ToObject<CoaliteResource>();
      }
      else
      {
        // In this case we loaded an existing coalite from a file and try to execute an action on it
        if (existingCoalite == null)
        {
          Console.WriteLine("Error: For actions other than CLAIM, an existing coalite resource file needs to be provided.");
          return;
        }
        coaliteResource = existingCoalite;
      }

      var request = coaliteResource.CreateActionRequest(rsa, coaliteAction, mintPayload, publicKeyData, userTag);

      var buffer = Encoding.UTF8.GetBytes(request.GetAsSignablePayload());
      var signature = Convert.FromBase64String(request.Signature);
      var clientRsa = RSA.Create();
      int bytesReadPk;
      var pk = request.SignerPublicKey.Split(' ').OrderByDescending(s => s.Length).FirstOrDefault();
      clientRsa.ImportRSAPublicKey(Convert.FromBase64String(pk), out bytesReadPk);

      // Check integrity of the signature of own request.
      if (!clientRsa.VerifyData(buffer, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
      {
        throw new Exception("Could not verify own signature. Something wrong with client keys.");
      }

      var requestStr = JToken.FromObject(request).ToString(Newtonsoft.Json.Formatting.None);
      var requestContent = new StringContent(requestStr,
                                             Encoding.UTF8, "application/json");
      var result = await httpClient.PostAsync($"{coaliteServerUri}/coalite/action", requestContent);
      var finalResult = await result.Content.ReadAsStringAsync();
      Console.WriteLine(finalResult);
      result.EnsureSuccessStatusCode();
    }
  }
}
