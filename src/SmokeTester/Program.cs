using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Tlabs.Config;

namespace Tlabs.Tools.Smoke {

  ///<summary>Smoke tester program (entry point)/></summary>
  public class Program {
    static ILogger log;
    static Program() {
      log= setupLog();
    }

    ///<summary>Main entry point/></summary>
    public static async Task<int> Main(string[] args) {
      await Task.Delay(10);

      using var tstCasesReader= openTestCasesReader(args);

      var ret= await TestRunner.ValidateTestCases(new TestCasesValidator(new TestCases(tstCasesReader)));

      Console.Out.Flush();
      return ret;
    }

    static StreamReader openTestCasesReader(string[] cmdArgs) {
      if (0 == cmdArgs.Length) {
        log.LogInformation("Reading from stdin...");
        return new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
      }

      if (cmdArgs.Any(arg => arg.StartsWith('-') || arg.StartsWith("/?"))) {
        Console.WriteLine($"*** Usage {Path.GetFileName(App.MainEntryPath??"?")} <text-cases-file>");
        return StreamReader.Null;
      }

      string filePath=   Path.IsPathRooted(cmdArgs[0])
                        ? cmdArgs[0]
                        : Path.Combine(Directory.GetCurrentDirectory(), cmdArgs[0]);
      log.LogInformation("Reading from: {path}", filePath);
      return new StreamReader(filePath, Encoding.UTF8, false);
    }

    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "Seems still to work")]
    static ILogger setupLog() {
      App.Setup= App.Setup with {
        LogFactory= ApplicationSetup.CreateConsoleLoggerFactory(new() {
          IncludeCategory= false,
          TimestampFormat= "HH:mm:ss.fff",   //shortend timestamp
          DfltMinimumLevel= LogLevel.Debug
        })
      };
      var logger= ApplicationSetup.InitLog<Program>();
      Console.Out.Flush();
      return logger;
    }

  }

}