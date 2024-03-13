
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Tlabs.Data.Model;
using Tlabs.Config;
using ProtoApp.Module.Data.Repo;

namespace ProtoApp.Module.Service {

  ///<summary>A module service</summary>
  ///<param name="userRepo">Data store repo for users as dependency.</param>
  public class ModuleService(ModuleObjRepo userRepo) {
    static ILogger log= Tlabs.App.Logger<ModuleService>();

    ///<summary>ModuleObj(s) from data store</summary>
    public List<Model.ModuleObj> ObjectList(QueryFilter? filter) {
      var query= userRepo.FilteredQuery(filter ?? new QueryFilter())
                         .Select(u => new Model.ModuleObj {
                           IntProperty= u.Id,
                           StrProperty= $"{u.UserName}"
                         });
      var objData= query.ToList();
      log.LogInformation("Returning {cnt} objects from data store.", objData.Count);
      return objData;
    }

    /// <summary>Module Service Configurator</summary>
    public class Configurator : IConfigurator<IServiceCollection> {
      /// <inheritdoc/>
      public void AddTo(IServiceCollection services, IConfiguration cfg) {
        services.AddScoped<ModuleObjRepo>();
        services.AddScoped<ModuleService>();

        log.LogInformation($"{nameof(ModuleService)} configured.");
      }
    }

  }
}