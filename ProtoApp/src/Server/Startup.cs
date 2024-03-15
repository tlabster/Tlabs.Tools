using System.Reflection;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Tlabs.Config;
using Microsoft.AspNetCore.Builder;

namespace ProtoApp {
  ///<summary>Server startup.</summary>
  public class Startup {

    ///<summary>Server startup main entry point.</summary>
    public static async Task Main(string[] args) {

      var webAppBuilder= new HostedWebAppBuilder(args);

      /* As an advanced option one could apply additional custom configuration
       * that has not already been applied through the appsettings configuration like:
       * (This shoul better go into the appsettings.Development.json...)
       */
      webAppBuilder.Configuration.AddUserSecrets(Assembly.GetEntryAssembly() ?? Assembly.Load(Tlabs.App.Setup.Name));

      var webApp= webAppBuilder.Build();

      /*
       * Additional API end-points could be programatically declared like:
       * (This should be better achieved with middleware configurators...)
       */
      webApp.MapGet("/hello", () => "Hello, world!");

      await webApp.RunAsync();
    }
  }

}
