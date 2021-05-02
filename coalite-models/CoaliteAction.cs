using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ketchup.Pizza.Models
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum CoaliteAction
  {
    EMIT,
    PUBLISH,
    CLAIM,
    TRANSFER, // TODO: Think
    ACCEPT,
    MINT,
    BURN
  }
}