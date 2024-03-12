using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;


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
    public static QueryCover<Model.ModuleObj> GetServiceObjects() {
      // var objData= Tlabs.App.FromScopedServiceInstance<ModuleService, IEnumerable<Model.ModuleObj>>((p, modSvc) => modSvc.ObjectList());
      return new QueryCover<Model.ModuleObj>(
        cover => Tlabs.App.FromScopedServiceInstance<ModuleService, IEnumerable<Model.ModuleObj>>((p, modSvc) => modSvc.ObjectList()),
        e => e.Message
      );
    }

    /// <summary>Module REST API Configurator</summary>
    public class Configurator : IConfigurator<MiddlewareContext> {
      /// <inheritdoc/>
      public void AddTo(MiddlewareContext middleCtx, IConfiguration cfg) {
        var app= (IEndpointRouteBuilder)middleCtx.AppBuilder;

        app.MapGet("/api/v1/module/objects/{id}", GetObject);
        app.MapGet("/api/v1/module/objects", GetServiceObjects);
      }
    }
  }
}