using System.Linq;


using Xunit;
using Xunit.Abstractions;
using Rieter.HMI.Test;
using System.IO;

namespace Rieter.Test.Test {

  public class TestRunnerTest {
    ITestOutputHelper tstout;
    public TestRunnerTest(ITestOutputHelper tstout) => this.tstout= tstout;

    [Fact]
    public void ParseCommandTest() {
      var binFile= "\"/binary/file path\"";
      var par1= "--par1";
      var par2= "'-p2 xyz'";
      var par3= "-x";

      var parsedCmd= TestRunner.parseCommand($"{binFile}  {par1} {par2}");
      Assert.Collection(parsedCmd,
                        e => Assert.Equal(e, binFile),
                        e => Assert.Equal(e, par1),
                        e => Assert.Equal(e, par2) );

      parsedCmd= TestRunner.parseCommand($"   {binFile}  {par1} {par2} {par3}  ");
      Assert.Collection(parsedCmd,
                        e => Assert.Equal(e, binFile),
                        e => Assert.Equal(e, par1),
                        e => Assert.Equal(e, par2),
                        e => Assert.Equal(e, par3));
    }


  }

}