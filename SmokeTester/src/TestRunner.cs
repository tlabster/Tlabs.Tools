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
      using var cts= new CancellationTokenSource();
      Task<SysCmdResult> cmdTsk;
      try {
        var cmd= new SystemCmd(parseCommand(tstValidator.TestCases.CommandLine))
                    .UseWorkingDir(App.Setup.ContentRoot);
        var cmdIO= new StdCmdIO();
        cmdTsk= cmd.Run(cmdIO, redirStdOut: true, redirStdIn: true, ctk: cts.Token);

        try {
          tstValidator.ValidateOutput(cmdIO.StdOut);
        }
        finally {
          var exitCode= await cmdTaskExit(cmdTsk, cts);
          tstValidator.ValidateExitCode(exitCode);
        }
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

    static async ValueTask<int> cmdTaskExit(Task<SysCmdResult> cmdTsk, CancellationTokenSource cts) {
      try {
        /* Would be great if we could signal the sub-process under test to terminate gracefully...
         */
        cts.CancelAfter(3000);
        using var res= await cmdTsk;
        return res.ExitCode;
      }
      catch (OperationCanceledException) {
        return 0;
      }
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

  }
}