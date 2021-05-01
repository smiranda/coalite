using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public static void SignCoalite(this Coalite coalite,
                                   RSACryptoServiceProvider signerRSA,
                                   CoaliteAction action,
                                   string actionPayload,
                                   string signerId)
    {
      var signature = new CoaliteSignature(action, actionPayload, signerId);
      var presignPayload = signature.GetPresignPayload();

      var coalitePayload = coalite.LoadPayload();
      var normalizedPayload = coalite.LoadPayloadAsString();
      var dataToSign = $"{coalite.Created.ToString()}{coalite.FullSecondStamp.ToString()}{coalite.Coalid}{normalizedPayload}{presignPayload}";
      var signatureBlob = Convert.ToBase64String(signerRSA
                                                 .SignData(Encoding.UTF8.GetBytes(dataToSign),
                                                           SHA256.Create()));
      signature.StoreSignature(signatureBlob);
      coalitePayload.Signatures.Add(signature);
      coalite.StorePayload(coalitePayload);
    }
  }
}