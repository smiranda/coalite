using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ketchup.Pizza.DB;
using Newtonsoft.Json.Linq;

namespace Ketchup.Pizza.Models
{
  public static class CoaliteSigningExtensions
  {
    public static string ToSignableString(this Coalite coalite)
    {
      var normalizedPayload = JToken.Parse(coalite.Payload).ToString(Newtonsoft.Json.Formatting.None);
      return $"{coalite.FullSecondStamp.ToString()}{coalite.Coalid}{normalizedPayload}";
    }
    public static string ToVerifiableString(this Coalite coalite)
    {
      // Does not include latest sig 
      // TODO: Should generalize this to all positions in the list ?
      var payload = JToken.Parse(coalite.Payload).ToObject<CoalitePayload>();
      payload.Signatures = payload.Signatures.SkipLast(1).ToList();
      var normalizedPayload = JToken.FromObject(payload).ToString(Newtonsoft.Json.Formatting.None);
      return $"{coalite.FullSecondStamp.ToString()}{coalite.Coalid}{normalizedPayload}";
    }
  }
}