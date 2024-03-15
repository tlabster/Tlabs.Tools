using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

using Tlabs;
using Tlabs.Misc;

namespace Tlabs.Tools.Smoke {

  ///<summary>Test cases validator</summary>
  public class TestCasesValidator {
    static readonly ILogger log= App.Logger<TestCasesValidator>();

    ///<summary>Ctor from <paramref name="tstCases"/></summary>
    public TestCasesValidator(TestCases tstCases) {
      this.TestCases= tstCases;
    }

    ///<summary>Test cases</summary>
    public TestCases TestCases { get; }

    ///<summary>Validate output stream</summary>
    public void ValidateOutput(TextReader input) {
      var defaultFails= TestCases.Failures[""];
      var failures= (name: "", cases: defaultFails);        //default failures
      var test= (name: "", cases: TestCases.Cases[""]);    //default cases

      var finalTestPassed= false;
      try {
        for (var line= input.ReadLineAsync().AwaitWithTimeout(3000); !finalTestPassed && null != line; line= input.ReadLineAsync().AwaitWithTimeout(3000)) {    //read all input lines
          var tstPassed= false;
          foreach (var tst in test.cases) if (true == (tstPassed= tst.Pattern.IsMatch(line))) {
            if (string.IsNullOrEmpty(tst.NextCase)) {
              finalTestPassed= true;
              log.LogInformation("Ultimate test case ({name}) passed.", test.name);
              break;
            }
            log.LogInformation("Test case '{name}' passed.", string.IsNullOrEmpty(test.name) ? "_INITIAL_" : test.name);
            test= (name: tst.NextCase, cases: TestCases.Cases[tst.NextCase]);                              //next test case
            failures= (name: tst.Failure, cases: defaultFails.Concat(TestCases.Failures[tst.Failure]));    //next failures
            break;
          }
          if (!tstPassed) foreach (var fail in failures.cases) {
            if (fail.Pattern.IsMatch(line)) throw new ValidationException($"'{failures.name}' failure detected\n ({line})");
          }
        }
      }
      catch (TimeoutException e) {
        log.LogWarning("Waiting for output expired ({msg})", e.Message);
      }
      if (!finalTestPassed) throw new ValidationException($"None of the final test case(s) [{string.Join(", ", TestCases.FinalCases.Select(p => p.Key))}] passed.");
    }

    /// <summary>Exit code validation.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification= "Not finished")]
    public void ValidateExitCode(int exitCode) {
      // if (!TestCases.ExpectedExitCodes.Contains(exitCode)) throw new ValidationException($"Bad exit code: {exitCode:D}");
      log.LogInformation("Process exit code: {code}", exitCode);
    }

    /// <summary>Test case validation exception.</summary>
    public class ValidationException : GeneralException {
      /// <summary>Default ctor</summary>
      public ValidationException() : base() { }

      /// <summary>Ctor from message</summary>
      public ValidationException(string message) : base(message) { }

      /// <summary>Ctor from inner exception <paramref name="e"/>.</summary>
      public ValidationException(Exception e) : base(e.Message, e) { }

      /// <summary>Ctor from <paramref name="msg"/> and inner exception <paramref name="e"/>.</summary>
      public ValidationException(string msg, Exception e) : base(msg, e) { }
    }

  }
}