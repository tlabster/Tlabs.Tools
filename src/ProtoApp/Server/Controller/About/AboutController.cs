using System;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using Tlabs.Config;
using Tlabs.Server.Model;

using ProtoApp.Server.Model;
using Tlabs.Misc;

namespace ProtoApp.Server.Controller {
  /// <summary>About controller</summary>
  /// <param name="conOut">Captured console output</param>
  [Route("api/v1/[controller]")]
  public class AboutController(ConsoleOutputDistributer conOut) : Tlabs.Server.Controller.ApiCtrl {
    static readonly ILogger log= Tlabs.App.Logger<AboutController>();

    ///<summary> About API </summary>
    [HttpGet]
    [AllowAnonymous]
    public ModelCover<About> About() => new ModelCover<About>(
      cover => {
        return new About(
          Version: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0",
          BuildDate: new DateTime(Build.CompileTime, DateTimeKind.Utc).ToString("o", DateTimeFormatInfo.InvariantInfo)
        );
      },
      e => resolveError(e)
    );

    ///<summary>Diagnostic log stream</summary>
    [HttpGet("diag")]
    public async Task LogStream(CancellationToken ctok) {
      using var ctokSrc= CancellationTokenSource.CreateLinkedTokenSource(ctok, Tlabs.App.AppLifetime.ApplicationStopping);
      Response.ContentType= "text/plain charset=utf-8";
      Response.StatusCode= (int)System.Net.HttpStatusCode.OK;
      var resStrm= Response.Body;
      conOut.AddStream(resStrm);
      log.LogDebug("Console log stream opened...");

      /* Awwait the closing of the (long running) request:
       */
      await ctokSrc.Token.AsTask(completeOnCancel: true);
      conOut.RemoveStream(resStrm);
      log.LogDebug("Console log stream closed.");
    }

  }
}