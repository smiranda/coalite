using System;
using System.Collections.Generic;
namespace Ketchup.Pizza.DB
{
  public class Coalite
  {
    public Coalite()
    {
    }

    public Coalite(string coalid,
                   string payload,
                   int fullSecondStamp)
    {
      Coalid = coalid;
      Payload = payload;
      FullSecondStamp = fullSecondStamp;
    }
    public Guid Id { get; set; }
    public string Coalid { get; set; }
    public string Payload { get; set; }
    public bool Claimed { get; set; }
    public DateTime Created { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public string ClaimedBy { get; set; }
    public int FullSecondStamp { get; set; }
  }
}