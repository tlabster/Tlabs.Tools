using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using Tlabs.Server;

namespace Rieter.HMI.Server {
  ///<summary>Server startup.</summary>
  public class OldFashionedStartup {

    ///<summary>Old-fashioned server startup entry point.</summary>
    public static async Task disable_Main(string[] args) {

      await ApplicationStartup.CreateServerHostBuilder(args)
                              .RunConsoleAsync();
    }
  }
}