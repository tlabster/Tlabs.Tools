using System.Linq;
using System.IO;

using Xunit;
using Xunit.Abstractions;

namespace Tlabs.Tools.Smoke.Test {

  public class TestCasesTest {
    ITestOutputHelper tstout;
    public TestCasesTest(ITestOutputHelper tstout) => this.tstout= tstout;

    [Fact]
    public void LoadTestCaseTest() {
      var tstCases= new TestCases(new StringReader(TST_CASE));
      Assert.NotEqual("?", tstCases.CommandLine);
      Assert.Single(tstCases.Failures.Keys);
      Assert.Equal(3, tstCases.Cases.Keys.Count());

      var f= tstCases.Failures[""].Single();
      Assert.StartsWith("default", f.Pattern.ToString());

      var c= tstCases.Cases[""].First();
      Assert.Equal("next", c.NextCase);
      Assert.Equal("onFail", c.Failure);
      Assert.EndsWith("pattern#1", c.Pattern.ToString());
      f= tstCases.Failures[c.Failure].First();
      Assert.Empty(f.Pattern.ToString());

      Assert.True(tstCases.Cases.ContainsKey("next"));
      Assert.True(tstCases.Cases["final"].All(c => string.IsNullOrEmpty(c.NextCase)));
      Assert.Contains(tstCases.FinalCases, p => "final" == p.Key);
    }

    [Fact]
    public void LoadTinyTest() {
      var tstCases= new TestCases(new StringReader(TINY_TST_CASE));
      Assert.NotEqual("?", tstCases.CommandLine);
      Assert.Single(tstCases.Failures.Keys);
      Assert.Equal(2, tstCases.Cases.Keys.Count());

      var f= tstCases.Failures[""].Single();
      Assert.StartsWith("default", f.Pattern.ToString());

      var c= tstCases.Cases[""].First();
      Assert.Equal("final", c.NextCase);
      Assert.True(tstCases.Cases["final"].All(c => string.IsNullOrEmpty(c.NextCase)));
      Assert.Contains(tstCases.FinalCases, p => "final" == p.Key);
    }

    [Fact]
    public void LoadMinimalTest() {
      var tstCases= new TestCases(new StringReader(MIN_TST_CASE));
      Assert.NotEqual("?", tstCases.CommandLine);
      Assert.Empty(tstCases.Failures.Keys);
      Assert.Single(tstCases.Cases);

      var f= tstCases.Failures[""].Single();
      Assert.Empty(f.Pattern.ToString());

      var c= tstCases.Cases[""].First();
      Assert.Empty(c.NextCase);
      Assert.StartsWith("final-", c.Pattern.ToString());
    }

    static string TST_CASE=
@"### Startup test cases:
#
# Start process:
cmd.exe /?
#
#Tests:


?:default-fail-pattern
>next?onFail:start-pattern#1
>next:start-pattern#2
next>next:next-pattern#1
next>final:next-pattern#2
final:final-pattern
";


    static string TINY_TST_CASE=
@"### Tiny test cases:
#
# Start process:
cmd.exe /?
#

?:default-fail-pattern
>final:start-pattern#1
final:final-pattern
";

    static string MIN_TST_CASE=
@"### Minimal test cases:
cmd.exe /?

:final-pattern
";
  }

}