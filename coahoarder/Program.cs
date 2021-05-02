using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Pizza.Models;
using Newtonsoft.Json.Linq;

namespace coahoarder
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var privateKeyData = File.ReadAllText("../keys/key.pem");
      var publicKeyData = File.ReadAllText("../keys/key.pub").Trim();

      var rsaProvider = new RSACryptoServiceProvider();
      rsaProvider.ImportFromPem(new ReadOnlySpan<char>(privateKeyData.ToCharArray()));

      var httpClient = new HttpClient();
      var response = await httpClient.GetStringAsync("http://localhost:5000/coalite");
      var coaliteResource = JToken.Parse(response).ToObject<CoaliteResource>();
      var request = coaliteResource.CreateActionRequest(rsaProvider, CoaliteAction.CLAIM, "", publicKeyData, "hoarder");
      var requestStr = JToken.FromObject(request).ToString(Newtonsoft.Json.Formatting.None);
      var requestContent = new StringContent(requestStr,
                                             Encoding.UTF8, "application/json");
      var result = await httpClient.PostAsync("http://localhost:5000/coalite/action", requestContent);
      result.EnsureSuccessStatusCode();
      var finalResult = await result.Content.ReadAsStringAsync();
      Console.WriteLine(finalResult);
    }
  }
}
