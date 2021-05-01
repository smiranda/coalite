using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ketchup.Pizza.DB;

namespace Ketchup.Pizza
{
  public class Startup
  {
    public Startup(IConfiguration configuration, ILogger<Startup> logger)
    {
      Configuration = configuration;
      _logger = logger;
    }

    public IConfiguration Configuration { get; }

    private ILogger<Startup> _logger;

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddMvc().AddNewtonsoftJson(options =>
      {
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
      });
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "coaliter", Version = "v1" });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //app.UseDeveloperExceptionPage();

      app.UseExceptionHandler("/error");// See ErrorController
                                        //app.UseStaticFiles(); -> wwwroot debugging

      app.UseCors(config =>
      {
        config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
      });

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "coaliter v1"));
      app.UseRouting();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
