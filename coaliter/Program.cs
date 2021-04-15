using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Ketchup.Pizza.Services;

namespace Ketchup.Pizza
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        WebHost.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((IConfigurationBuilder builder) =>
                    {
                      builder
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", false, false)
                      .AddJsonFile("appsettings.local.json", true, false)
                      .AddJsonFile("credentials.json", true, false)
                      .AddJsonFile("credentials.local.json", true, false)
                      .AddEnvironmentVariables()
                      .Build();
                    })
           .UseStartup<Startup>().ConfigureLogging(logging =>
            {
              logging.ClearProviders();
              logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            })
            .ConfigureServices((IServiceCollection services) =>
            {
              services.AddSingleton<ICoaliter, Coaliter>();
              services.AddTransient<IHostedService, Coaliter>(x => (Coaliter)x.GetRequiredService<ICoaliter>());
            })
            .UseNLog().Build().Run();
      }
      finally
      {
        NLog.LogManager.Shutdown();
      }
    }
  }
}
