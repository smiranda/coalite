using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Ketchup.Pizza.Models
{
  public class CoaliteResource
  {
    public CoaliteResource(string coalid,
                           long seqid,
                           List<CoaliteSignature> signatures,
                           DateTime timestamp)
    {
      Coalid = coalid;
      Signatures = signatures;
      Seqid = seqid;
      Timestamp = timestamp;
    }
    public CoaliteActionRequest CreateActionRequest(RSACryptoServiceProvider signerRSA,
                                                    CoaliteAction action,
                                                    string actionPayload,
                                                    string signerPublicKey,
                                                    string signerId)
    {
      var request = new CoaliteActionRequest(this,
                                             signerPublicKey,
                                             action,
                                             actionPayload,
                                             signerId);
      // Create action signature
      var signature = new CoaliteSignature(action,
                                           actionPayload,
                                           signerPublicKey,
                                           signerId);
      var presignPayload = signature.GetPresignPayload();
      var dataToSign = request.Coalite.GetAsSignablePayload(presignPayload);
      request.ActionSignature = Convert.ToBase64String(signerRSA
                                       .SignData(Encoding.UTF8.GetBytes(dataToSign),
                                                 SHA256.Create()));

      // Create request signature
      dataToSign = request.GetAsSignablePayload();
      request.Signature = Convert.ToBase64String(signerRSA
                                       .SignData(Encoding.UTF8.GetBytes(dataToSign),
                                                 SHA256.Create()));
      return request;
    }

    public string GetAsSignablePayload(string additionalPayload)
    {
      var normalizedPayload = JToken.FromObject(Signatures).ToString(Newtonsoft.Json.Formatting.None);
      return $"{Seqid.ToString()}{Coalid}{normalizedPayload}{additionalPayload}";
    }
    public string Coalid { get; set; }
    public List<CoaliteSignature> Signatures { get; set; }
    public long Seqid { get; }
    public DateTime Timestamp { get; }
  }
}