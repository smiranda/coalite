using System.ComponentModel.DataAnnotations;

namespace Ketchup.Pizza.Models
{
  public class CoaliteActionRequest
  {
    public CoaliteActionRequest()
    {
    }

    public CoaliteActionRequest(CoaliteResource coalite,
                                string signerPublicKey,
                                CoaliteAction action,
                                string actionPayload,
                                string signerId)
    {
      Coalite = coalite;
      SignerPublicKey = signerPublicKey;
      Action = action;
      ActionPayload = actionPayload;
      SignerId = signerId;
    }

    public CoaliteResource Coalite { get; set; }
    [StringLength(1000)]
    public string SignerPublicKey { get; set; }
    public CoaliteAction Action { get; set; }
    [StringLength(1000000)]
    public string ActionPayload { get; set; }
    [StringLength(1000)]
    public string SignerId { get; set; }
    [StringLength(1000)]
    public string ActionSignature { get; set; }
    [StringLength(1000)]
    public string Signature { get; set; }
    public string GetAsSignablePayload()
    {
      return Coalite.GetAsSignablePayload($"{SignerPublicKey}{Action.ToString()}{ActionPayload}{SignerId}");
    }
  }
}