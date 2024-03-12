using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tlabs.Data.Store {

  ///<summary>SQLite configurator.</summary>
  public class SqliteConfigurator : DbServerConfigurator<AggregatingDStoreCtxConfigurator> {
    static System.Data.Common.DbConnection? __memConn;

    ILogger log= Tlabs.App.Logger<SqliteConfigurator>();
    string? providerAssembly;
    string? migrationAssemblyName;

    ///<summary>Default ctor.</summary>
    public SqliteConfigurator() : base() { }
    ///<summary>Ctor from <paramref name="config"/>.</summary>
    public SqliteConfigurator(IDictionary<string, string> config) : base(config) { }

    ///<inheritdoc/>
    protected override void configureDbOptions(DbContextOptionsBuilder<DStoreContext<AggregatingDStoreCtxConfigurator>> opt) {
      this.providerAssembly= typeof(SqliteDbContextOptionsBuilderExtensions).Assembly.GetName().Name;
      this.config.TryGetValue("migrationAssembly", out this.migrationAssemblyName);
      migrationAssemblyName??= GetType().Assembly.GetName().Name;
      var binPath= new DirectoryInfo(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty);   //App.MainEntryPath));

      if (this.connStr.Contains("Mode=Memory") || this.connStr.Contains("=:memory:")) {
        __memConn= new Microsoft.Data.Sqlite.SqliteConnection(this.connStr);
        __memConn.Open();   //keep in-memory db open
        opt.UseSqlite(__memConn, ctxOptionBuilder);
        log.LogInformation("Using permanent in-memory db-connection.");
      }
      else opt.UseSqlite(connStr, ctxOptionBuilder);

      if (App.Setup.IsDevelopmentEnv) {
        opt.ConfigureWarnings(warn => warn.Log(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
        opt.EnableSensitiveDataLogging(true);
        log.LogWarning("Sensitive data logging enabled.");
      }
    }
    void ctxOptionBuilder(Microsoft.EntityFrameworkCore.Infrastructure.SqliteDbContextOptionsBuilder optBuilder) {
      optBuilder.CommandTimeout(4100)
                .MigrationsAssembly(migrationAssemblyName)
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery); //QuerySplittingBehavior.SplitQuery
      log.LogInformation("{provider} provider configured with migration assembly: {migration}", providerAssembly, migrationAssemblyName);
    }

  }

}