using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

using Tlabs.Config;
using System;

namespace Tlabs.Data.Store {

  ///<summary>Design time <see cref="DStoreContext{T}"/> factory used with <c>dotnet ef migrations</c>.</summary>
  public class DesignTimeAppDbCtxFactory : IDesignTimeDbContextFactory<DStoreContext<AggregatingDStoreCtxConfigurator>> {
    /// <inheritdoc/>
    public DStoreContext<AggregatingDStoreCtxConfigurator> CreateDbContext(string[] args) {
      App.Setup= App.Setup with {
        ContentRoot= Environment.CurrentDirectory
      };
      App.Setup.Configuration.AddApplicationConfig();

      var svcColl= new ServiceCollection()
        .AddLogging(log => log.AddConsole())
        .ApplyConfigurators(Tlabs.App.Settings, "applicationServices");

      var svcProvider= svcColl.BuildServiceProvider();
      var scopedSvcProv= svcProvider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;
      return scopedSvcProv.GetRequiredService<DStoreContext<AggregatingDStoreCtxConfigurator>>();
    }
  }


}