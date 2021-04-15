using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Ketchup.Pizza.DB
{
  public interface IDbContextDefaults
  {
    string SQLUUIDDefault { get; }
    string SQLDateDefault { get; }
    string SQLJsonDataType { get; }
  }

  public class GenericDbDefaults : IDbContextDefaults
  {
    public string SQLUUIDDefault => throw new NotImplementedException();

    public string SQLDateDefault => throw new NotImplementedException();

    public string SQLJsonDataType => throw new NotImplementedException();
  }

  public class SqliteDbDefaults : IDbContextDefaults
  {
    public string SQLUUIDDefault => "(upper(hex(randomblob(4))) || '-' || upper(hex(randomblob(2))) || '-4' || substr(upper(hex(randomblob(2))),2) || '-' || substr('89AB',abs(random()) % 4 + 1, 1) || substr(upper(hex(randomblob(2))),2) || '-' || upper(hex(randomblob(6))))";

    public string SQLDateDefault => "date('now')";

    public string SQLJsonDataType => "string";
  }

  public class PostgresDbDefaults : IDbContextDefaults
  {
    public string SQLUUIDDefault => "uuid_generate_v1()";

    public string SQLDateDefault => "now()";

    public string SQLJsonDataType => "jsonb";
  }
}
