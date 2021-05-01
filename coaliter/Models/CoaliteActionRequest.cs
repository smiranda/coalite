using System;
using System.ComponentModel.DataAnnotations;
using Ketchup.Pizza.DB;
using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CoaliteActionRequest
  {
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
    public string Signature { get; set; }
    public string GetAsSignablePayload()
    {
      return Coalite.GetAsSignablePayload($"{SignerPublicKey}{Action.ToString()}{ActionPayload}{SignerId}");
    }
  }
}