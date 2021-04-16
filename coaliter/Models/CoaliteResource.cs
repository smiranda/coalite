using System;
using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CoaliteResource
  {
    public CoaliteResource(string coalid,
                           string payload,
                           string signature,
                           long seqid,
                           DateTime timestamp)
    {
      Coalid = coalid;
      Payload = payload;
      Signature = signature;
      Seqid = seqid;
      Timestamp = timestamp;
    }

    public string Coalid { get; set; }
    public string Payload { get; set; }
    public string Signature { get; set; }
    public long Seqid { get; }
    public DateTime Timestamp { get; }
  }
}