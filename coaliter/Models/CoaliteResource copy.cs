using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CoaliteResource
  {
    public CoaliteResource(string coalid, string payload, string signature)
    {
      Coalid = coalid;
      Payload = payload;
      Signature = signature;
    }

    public string Coalid { get; set; }
    public string Payload { get; set; }
    public string Signature { get; set; }
  }
}