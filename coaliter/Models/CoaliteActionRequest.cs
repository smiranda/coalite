using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CoaliteActionRequest
  {
    [StringLength(36)]
    public string Coalid { get; set; }
    [StringLength(1000)]
    public string SignerPublicKey { get; set; }
    public CoaliteAction Action { get; set; }
    [StringLength(1000000)]
    public string ActionPayload { get; set; }
    [StringLength(1000)]
    public string SignerId { get; set; }
    [StringLength(1000)]
    public string Signature { get; set; }
    public string GetAsSignablePayload()
    {
      return $"{Coalid}{SignerPublicKey}{Action.ToString()}{ActionPayload}{SignerId}";
    }
  }
}