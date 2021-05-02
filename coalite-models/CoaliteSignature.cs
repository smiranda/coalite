using System;
using System.ComponentModel.DataAnnotations;

namespace Ketchup.Pizza.Models
{
  public class CoaliteSignature
  {
    public CoaliteSignature(CoaliteAction action,
                            string actionPayload,
                            string signerPublicKey,
                            string signerId)
    {
      Action = action;
      ActionPayload = actionPayload;
      SignerPublicKey = signerPublicKey;
      SignerId = signerId;
    }

    public string GetPresignPayload()
    {
      return $"{Action.ToString()}{SignerPublicKey}{SignerId}{ActionPayload}";
    }

    public void StoreSignature(string signature)
    {
      Signature = signature;
    }

    public override bool Equals(object obj)
    {
      return obj is CoaliteSignature signature &&
             Signature == signature.Signature &&
             Action == signature.Action &&
             ActionPayload == signature.ActionPayload &&
             SignerPublicKey == signature.SignerPublicKey &&
             SignerId == signature.SignerId;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Signature, Action, ActionPayload, SignerPublicKey, SignerId);
    }

    [StringLength(1000)]
    public string Signature { get; set; }
    public CoaliteAction Action { get; set; }
    [StringLength(1000000)]
    public string ActionPayload { get; set; }
    [StringLength(1000)]
    public string SignerPublicKey { get; set; }
    [StringLength(1000)]
    public string SignerId { get; }
  }
}