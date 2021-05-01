using System;
using System.ComponentModel.DataAnnotations;

namespace Ketchup.Pizza.Models
{
  public class CoaliteSignature
  {
    public CoaliteSignature(CoaliteAction action,
                            string actionPayload,
                            string signerId)
    {
      Action = action;
      ActionPayload = actionPayload;
      SignerId = signerId;
    }

    public string GetPresignPayload()
    {
      return $"{Action.ToString()}{SignerId}{ActionPayload}";
    }

    public void StoreSignature(string signature)
    {
      Signature = signature;
    }

    [StringLength(1000)]
    public string Signature { get; set; }
    [StringLength(64)]
    public CoaliteAction Action { get; set; }
    [StringLength(1000000)]
    public string ActionPayload { get; set; }
    public string SignerId { get; set; }
  }
}