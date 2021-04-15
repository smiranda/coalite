using Microsoft.AspNetCore.Http;

namespace Ketchup.Pizza.Models
{
  public class CountResource
  {
    public CountResource(int count)
    {
      Count = count;
    }

    public int Count { get; set; }
  }
}