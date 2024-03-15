using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;


using Tlabs.Timing;
using Tlabs.Config;
using Tlabs.Server.Model;
using ProtoApp.Module.Service;

namespace ProtoApp.Module.API {

  /// <summary>REST API of the Module</summary>
  public class ModuleApi {
    static ILogger log= Tlabs.App.Logger<ModuleApi>();

    /// <summary>Get object with <paramref name="id"/></summary>
    public static ModelCover<Model.ModuleObj> GetObject(int id) => new ModelCover<Model.ModuleObj>(
      cover => new Model.ModuleObj { IntProperty= id },
      e => e.Message
    );

    /// <summary>Get object(s) from (scoped) ModuleService</summary>
    public static QueryCover<Model.ModuleObj> GetFilteredServiceObjects(FilterParam<Model.ModuleObj>? param) => new QueryCover<Model.ModuleObj>(
      cover => Tlabs.App.FromScopedServiceInstance<ModuleService, IEnumerable<Model.ModuleObj>>((p, modSvc) => modSvc.ObjectList(param?.AsQueryFilter())),
      e => e.Message
    );

    /// <summary>Module REST API Configurator</summary>
    public class Configurator : IConfigurator<MiddlewareContext> {
      static ClockedRunner? clkRunner;
      /// <inheritdoc/>
      public void AddTo(MiddlewareContext middleCtx, IConfiguration cfg) {
        var app= middleCtx.AsWebApplication();

        app.MapGet("/api/v1/module/objects/{id}", GetObject);
        log.LogInformation("Configured endpoint: {endPoint}", "/api/v1/module/objects/{id}");

        app.MapGet("/api/v1/module/objects", GetFilteredServiceObjects);
        log.LogInformation("Configured endpoint: {endPoint}", "/api/v1/module/objects");


        clkRunner= new ClockedRunner($"{Tlabs.App.Setup.Name}-hartbeat", 7891, (ctk) => {
          log.LogInformation("{name} alive and knocking every {ms} ms", clkRunner!.Title, clkRunner!.ClockInterval);
          return false;
        });

      }
    }
  }
}