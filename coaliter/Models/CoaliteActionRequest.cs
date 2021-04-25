using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CoaliteActionRequest
  {
    [StringLength(1000000)]
    public string payload { get; set; }
    [StringLength(36)]
    public string Coalid { get; set; }
    public string SignerPublicKey { get; set; }
    public CoaliteSignature Signature { get; set; }
  }
}