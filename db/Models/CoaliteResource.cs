using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ketchup.Pizza.Models
{
  public class CoaliteResource
  {
    public CoaliteResource(string coalid,
                           long seqid,
                           List<CoaliteSignature> signatures,
                           string payload,
                           DateTime timestamp)
    {
      Coalid = coalid;
      Signatures = signatures;
      Seqid = seqid;
      Payload = payload;
      Timestamp = timestamp;
    }
    public string GetAsSignablePayload(string additionalPayload)
    {
      var normalizedPayload = JToken.FromObject(Signatures).ToString(Newtonsoft.Json.Formatting.None);
      return $"{Seqid.ToString()}{Coalid}{normalizedPayload}{additionalPayload}";
    }
    public string Coalid { get; set; }
    public List<CoaliteSignature> Signatures { get; set; }
    public long Seqid { get; }
    public string Payload { get; }
    public DateTime Timestamp { get; }
  }
}