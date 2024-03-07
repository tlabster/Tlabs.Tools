using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tlabs.Data.Store {

  ///<summary>SQLite configurator.</summary>
  public class SqliteConfigurator : DbServerConfigurator<AggregatingDStoreCtxConfigurator> {
    static System.Data.Common.DbConnection? __memConn;

    ILogger log= Tlabs.App.Logger<SqliteConfigurator>();
    string? providerAssembly;
    string? migrationAssemblyName;

    ///<summary>Application binary path.</summary>
    public const string MAIN_ENTRY_PATH= "{%MainEntryPath%}";
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
      this.connStr= this.connStr.Replace(MAIN_ENTRY_PATH, binPath.FullName);

      var svcColl= (opt as IDbServerCfgContextOptionsBuilder)?.SvcCollection;
      svcColl?.AddSingleton(new SqliteConnectionInfo(this.connStr));           //publish connection string info.

      if (this.connStr.Contains("Mode=Memory") || this.connStr.Contains("=:memory:")) {
        __memConn= new Microsoft.Data.Sqlite.SqliteConnection(this.connStr);
        __memConn.Open();   //keep in-memory db open
        opt.UseSqlite(__memConn, ctxOptionBuilder);
        log.LogInformation("Using permanent in-memory db-connection.");
      }
      else opt.UseSqlite(connStr, ctxOptionBuilder);

#if DEBUG
      opt.ConfigureWarnings(warn => warn.Log(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
      opt.EnableSensitiveDataLogging(true);
      log.LogWarning("Sensitive data logging enabled.");
#endif
    }
    void ctxOptionBuilder(Microsoft.EntityFrameworkCore.Infrastructure.SqliteDbContextOptionsBuilder optBuilder) {
      optBuilder.CommandTimeout(3600)
                .MigrationsAssembly(migrationAssemblyName)
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery); //QuerySplittingBehavior.SplitQuery
      log.LogInformation("{provider} provider configured with migration assembly: {migration}", providerAssembly, migrationAssemblyName);
    }

  }

  ///<summary>Sqlite specific.</summary>
  public class AggregatingSqliteDStoreCtxConfigurator : AggregatingDStoreCtxConfigurator {
    ///<summary>Ctor from <paramref name="modelConfigs"/>.</summary>
    public AggregatingSqliteDStoreCtxConfigurator(IEnumerable<IDStoreConfigModel> modelConfigs) : base(modelConfigs) { }
  }

  ///<summary>Sqlite connection info</summary>
  public class SqliteConnectionInfo {
    static readonly Regex DS_PATTERN= new("DataSource=([^;]*)(;|$)");

    ///<summary>Ctor from <paramref name="connString"/></summary>
    public SqliteConnectionInfo(string connString) {
      this.ConnString= connString;
      this.DataSource= DS_PATTERN.Match(connString).Groups[1].Value;
    }

    ///<summary>Db connetion string</summary>
    public string ConnString { get; }
    ///<summary>DataSource</summary>
    public string DataSource { get; }
  }

}