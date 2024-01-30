using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Tlabs;
using Tlabs.Misc;

namespace Rieter.HMI.Test {

  ///<summary>Test cases</summary>
  public class TestCases {
    static readonly ILogger log= App.Logger<TestCases>();
    static TestCases? loadedTest;
    DictionaryList<string, FailurePattern> failures= new(k => handleMissingKey(k, FailurePattern.Default), StringComparer.OrdinalIgnoreCase);
    DictionaryList<string, Case> cases= new(k => handleMissingKey(k, Case.Default), StringComparer.OrdinalIgnoreCase);

    static IEnumerable<T> handleMissingKey<T>(string k, T dflt) {
      int lineNo= loadedTest?.Cases.Values.Where(c => k.Equals(c.Failure) || k.Equals(c.NextCase)).Select(c => c.LineNo).FirstOrDefault() ?? 0;
      log.LogWarning("Udefined {type} ({key}) referenced in line: {ln}", typeof(T).Name, k, lineNo);
      return EnumerableUtil.One(dflt);
    }

    ///<summary>Ctor from <paramref name="caseReader"/></summary>
    public TestCases(TextReader caseReader) {
      TestCases.loadedTest= this;
      this.CommandLine= tinyTestCaseParser(caseReader);
      this.FinalCases= Cases.Where(p => p.Value.All(c => string.IsNullOrEmpty(c.NextCase))).ToList();
      if (!this.FinalCases.Any()) throw new InvalidOperationException("No final test case(s) defined.");
    }

    ///<summary>Command line</summary>
    public string CommandLine { get; }= "?";
    ///<summary>Failure tests</summary>
    public IReadOnlyDictList<string, FailurePattern> Failures => failures;
    ///<summary>Test cases</summary>
    public IReadOnlyDictList<string, Case> Cases => cases;

    ///<summary>List of final case(s)</summary>
    public IList<KeyValuePair<string, IEnumerable<Case>>> FinalCases { get; }

    ///<summary>List of exspected exit code(s)</summary>
    public static IEnumerable<int> ExpectedExitCodes => EnumerableUtil.One(0);

    /* Parse test cases into the Cases DictiobaryList<> and return the command line
     * for the program to be tested.
     */
    string tinyTestCaseParser(TextReader caseReader) {
      string? cmd= null;

      /* For each line in text stream:
       */
      for (string? line= readCaseLine(caseReader); null != line; line= readCaseLine(caseReader)) {

        if (null == cmd) {    //first line is command
          cmd= line.Trim();
          continue;
        }

        /* Parse pattern (must exist in every line)
         */
        int p= line.IndexOf(':');
        if (p < 0) throw new InvalidOperationException($"No pattern found in test case line: {TestCases.parsedLineNo:D}:{line}");
        string pattern=   p == line.Length
                        ? ""
                        : line[(p+1)..];
        line= line[0..p];

        /* Parse optional test case name
         */
        string? testName= null;
        p= line.IndexOf('>');
        if (p >= 0) {
          testName= line[0..p];
          line=   p == line.Length
                ? ""
                : line[(p+1)..];
        }

        var tst= parseNextFail(line);
        if (0 == tst.next.Length && null != tst.fail) failures.Add(tst.fail, new(pattern, parsedLineNo));               //failure definition
        else cases.Add(testName ?? tst.next, new(null == testName ? "" : tst.next, tst.fail??"", pattern, parsedLineNo));   //test case definition
      }
      return cmd ?? "?";
    }

    /* Parse next case and failure (both are optional)
     */
    static (string next, string? fail) parseNextFail(string line) {
      var part= line.Split('?', 2, StringSplitOptions.TrimEntries);
      return (next: part[0], fail: part.Length > 1 ? part[1] : null);
    }

    static int parsedLineNo;
    static string? readCaseLine(TextReader caseReader) {    //read next non empty & non comment line:
      string? line;
      for (line= caseReader.ReadLine();
           null != line;
           line= caseReader.ReadLine()) {
        ++parsedLineNo;
        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#')) continue;    //skip this line
        break;
      }
      return line;
    }

    ///<summary>Failure pattern</summary>
    public class FailurePattern {
      const RegexOptions xopt=   RegexOptions.IgnoreCase
                               | RegexOptions.Singleline
                               | RegexOptions.CultureInvariant;
      ///<summary>Default <see cref="FailurePattern"/></summary>
      public static readonly FailurePattern Default= new("", 0);

      ///<summary>Ctor from <paramref name="pattern"/> and <paramref name="lno"/></summary>
      public FailurePattern(string pattern, int lno) {
        this.Pattern= new Regex(pattern, xopt);
        this.LineNo= lno;
      }

      ///<summary>Test pattern</summary>
      public Regex Pattern { get; }
      ///<summary>Line no. pattern definition</summary>
      public int LineNo { get; }
    }

    ///<summary>Test case</summary>
    public class Case : FailurePattern {
      ///<summary>Default <see cref="Case"/></summary>
      public static new readonly Case Default= new("", "", "", 0);

      ///<summary>Ctor from <paramref name="next"/>, <paramref name="fail"/> and <paramref name="pattern"/></summary>
      public Case(string next, string fail, string pattern, int lno) : base(pattern, lno) {
        this.NextCase= next;
        this.Failure= fail;
      }
      ///<summary>Next case name or null if final (success) case</summary>
      public string NextCase { get; init; }
      ///<summary>Optional new failure category</summary>
      public string Failure { get; init; }
    }
  }
}