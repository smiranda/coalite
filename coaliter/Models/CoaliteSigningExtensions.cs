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
    public static void AppendSignedAction(this Coalite coalite,
                                          CoaliteAction action,
                                          string actionPayload,
                                          string signerPublicKey,
                                          string signerId,
                                          string signatureBlob)
    {
      var signature = new CoaliteSignature(action,
                                           actionPayload,
                                           signerPublicKey,
                                           signerId);
      signature.StoreSignature(signatureBlob);

      var coalitePayload = coalite.LoadPayload();
      coalitePayload.Signatures.Add(signature);
      coalite.StorePayload(coalitePayload);
    }

    public static void SignCoalite(this Coalite coalite,
                                   RSACryptoServiceProvider signerRSA,
                                   CoaliteAction action,
                                   string actionPayload,
                                   string signerPublicKey,
                                   string signerId)
    {
      var signature = new CoaliteSignature(action, actionPayload, signerPublicKey, signerId);
      var presignPayload = signature.GetPresignPayload();

      var coalitePayload = coalite.LoadPayload();
      var dataToSign = coalite.GetAsSignablePayload(presignPayload);
      var signatureBlob = Convert.ToBase64String(signerRSA
                                                 .SignData(Encoding.UTF8.GetBytes(dataToSign),
                                                           SHA256.Create()));
      signature.StoreSignature(signatureBlob);
      coalitePayload.Signatures.Add(signature);
      coalite.StorePayload(coalitePayload);
    }
  }
}