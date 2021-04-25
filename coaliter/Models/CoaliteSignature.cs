using System;
using System.ComponentModel.DataAnnotations;

namespace Ketchup.Pizza.Models
{
  public class CoaliteSignature
  {
    public CoaliteSignature(string signature,
                            CoaliteAction action,
                            string actionPayload,
                            string signerId)
    {
      Signature = signature;
      Action = action;
      ActionPayload = actionPayload;
      SignerId = signerId;
    }

    [StringLength(1000)]
    public string Signature { get; set; }
    [StringLength(64)]
    public CoaliteAction Action { get; set; }
    [StringLength(10000)]
    public string ActionPayload { get; set; }
    public string SignerId { get; set; }
  }
}