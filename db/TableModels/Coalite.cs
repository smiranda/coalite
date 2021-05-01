using System;
using System.Collections.Generic;
using System.Linq;
using Ketchup.Pizza.Models;
using Newtonsoft.Json.Linq;

namespace Ketchup.Pizza.DB
{
  public class Coalite
  {
    public Coalite()
    {
    }

    public Coalite(string coalid,
                   string payload,
                   int fullSecondStamp)
    {
      Coalid = coalid;
      Payload = payload;
      FullSecondStamp = fullSecondStamp;
    }

    public CoalitePayload LoadPayload()
    {
      return JToken.Parse(Payload).ToObject<CoalitePayload>();
    }

    public string LoadPayloadAsString()
    {
      return JToken.Parse(Payload).ToString(Newtonsoft.Json.Formatting.None);
    }

    public void StorePayload(CoalitePayload payload)
    {
      Payload = JToken.FromObject(payload).ToString(Newtonsoft.Json.Formatting.None);
    }

    public string GetAsSignablePayload(string additionalPayload)
    {
      var normalizedPayload = LoadPayloadAsString();
      return $"{FullSecondStamp.ToString()}{Coalid}{normalizedPayload}{additionalPayload}";
    }

    public bool EqualToResource(CoaliteResource coaliteResource)
    {
      var deserializedPayload = LoadPayload();
      return (coaliteResource.Coalid == Coalid &&
              coaliteResource.Payload == Payload &&
              coaliteResource.Seqid == FullSecondStamp &&
              coaliteResource.Signatures.SequenceEqual(deserializedPayload.Signatures));
    }

    public Guid Id { get; set; }
    public string Coalid { get; set; }
    public string Payload { get; set; }
    public bool Claimed { get; set; }
    public DateTime Created { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public string ClaimedBy { get; set; }
    public int FullSecondStamp { get; set; }
  }
}