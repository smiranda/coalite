using System.Collections.Generic;

namespace Ketchup.Pizza.Models
{
  public class CoalitePayload
  {
    public CoalitePayload()
    {
    }
    public CoalitePayload(List<CoaliteSignature> signatures)
    {
      Signatures = signatures;
    }
    public List<CoaliteSignature> Signatures { get; set; }
  }
}