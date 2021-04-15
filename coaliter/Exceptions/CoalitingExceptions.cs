using System;
namespace Ketchup.Pizza.Services
{
  public class CoalitingException : Exception
  {
    public CoalitingException(int statusCode)
    {
      Data["StatusCode"] = statusCode;
    }
    public CoalitingException(int statusCode, string message)
        : base(message)
    {
      Data["StatusCode"] = statusCode;
    }
    public CoalitingException(int statusCode, string message, Exception inner)
        : base(message, inner)
    {
      Data["StatusCode"] = statusCode;
    }
  }
}