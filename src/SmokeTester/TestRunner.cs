using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tlabs;
using Tlabs.Sys;

namespace Tlabs.Tools.Smoke {

  ///<summary>Test runner</summary>
  public partial class TestRunner {
    static readonly ILogger log= App.Logger<TestRunner>();

    ///<summary>Vaildate <paramref name="tstValidator"/></summary>
    public static async Task<int> ValidateTestCases(TestCasesValidator tstValidator) {
      // using var cts= new CancellationTokenSource(20_000);
      using var cts= new CancellationTokenSource();
      Task<SysCmdResult> cmdTsk;
      try {
        var cmd= new SystemCmd(parseCommand(tstValidator.TestCases.CommandLine));
        var cmdIO= new StdCmdIO();
        cmdTsk= cmd.Run(cmdIO, redirStdOut: true, redirStdIn: true, ctk: cts.Token);

        int exitCode= 0;
        try {
          tstValidator.ValidateOutput(cmdIO.StdOut);
        }
        finally {
          try {
            await trySignalCmdTermination(cmdTsk, cmdIO);
            cts.CancelAfter(3000);
            using var res= await cmdTsk;
            exitCode= res.ExitCode;
          }
          catch (OperationCanceledException) {
            exitCode= -999;
          }
        }
        tstValidator.ValidateExitCode(exitCode);
      }
      catch (TestCasesValidator.ValidationException vx) {
        log.LogWarning("Validation test failed: {msg}", vx.Message);
        return 1;
      }
      catch (Exception e) {
        log.LogError(e, "Program execution failed");
        return -1;
      }
      return 0;
    }

    [GeneratedRegex(@"((?<![\\])['""])((?:.(?!(?<! [\\])\1))*.?)\1")]
    private static partial Regex QUOTEDregex();
    static readonly Regex QUOTED_REX= QUOTEDregex();
    const StringSplitOptions SPLIT_OPT= StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    internal static string[] parseCommand(string cmd) {
      int p= 0;
      var cmdLst= new List<string>();
      foreach (Match match in QUOTED_REX.Matches(cmd)) {
        cmdLst.AddRange(cmd[p..match.Index].Split(' ', SPLIT_OPT));
        cmdLst.Add(match.Value);
        p= match.Index + match.Length;
      }
      cmdLst.AddRange(cmd[p..].Split(' ', SPLIT_OPT));
      return cmdLst.ToArray();
    }

    static async Task trySignalCmdTermination(Task<SysCmdResult> cmdTsk, StdCmdIO cmdIO) {
      if (!cmdTsk.IsCompleted) {
        await Task.Delay(300);
        if (!cmdTsk.IsCompleted) try {
          log.LogDebug("Trying to stop command with ctrl-C");
          cmdIO.StdIn.WriteLine("\x04\x03q");  //try stopping process with Ctrl-C
          cmdIO.StdIn.WriteLine("exit");
          cmdIO.StdIn.WriteLine("stop");
          cmdIO.StdIn.Close();
        }
        catch (Exception) {
          log.LogWarning("Failed to signal command termination");
        }
      }
    }
  }
}