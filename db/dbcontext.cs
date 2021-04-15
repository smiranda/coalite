using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography;

namespace Ketchup.Pizza.DB
{
  public class APIMDBContextFactory : IDesignTimeDbContextFactory<CoaliteDBContext>
  {
    public CoaliteDBContext CreateDbContext(string[] args)
    {
      var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("credentials.json", false)
            .AddJsonFile("credentials.local.json", true)
            .AddEnvironmentVariables()
            .Build();

      var connectionString = configuration.GetConnectionString("Connection");
      var optionsBuilder = new DbContextOptionsBuilder<CoaliteDBContext>();
      optionsBuilder.UseSqlite(connectionString);
      return new CoaliteDBContext(new SqliteDbDefaults(), optionsBuilder.Options);
    }
  }
  public interface ICoaliteDBContext { };
  public class CoaliteDBContext : DbContext, ICoaliteDBContext
  {
    public IDbContextDefaults _defaults;
    public CoaliteDBContext(DbContextOptions<CoaliteDBContext> options)
        : base(options)
    {
    }
    public CoaliteDBContext(IDbContextDefaults defaults,
                            DbContextOptions<CoaliteDBContext> options) : base(options)
    {
      _defaults = defaults;
    }
    public DbSet<Coalite> Coalites { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Coalite>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasDefaultValueSql(_defaults.SQLUUIDDefault);
        entity.Property(e => e.Claimed)
              .HasDefaultValue(false);
        entity.HasIndex(e => e.Coalid)
              .IsUnique();
        entity.HasIndex(e => e.FullSecondStamp)
              .IsUnique();
        entity.Property(e => e.Created)
              .HasDefaultValueSql(_defaults.SQLDateDefault);
      });
    }
  }
}