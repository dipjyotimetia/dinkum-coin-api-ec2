using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace DinkumCoin.Api.PactVerify.Framework
{

    public class XUnitOutput : IOutput
    {
        private readonly ITestOutputHelper _output;

        public XUnitOutput(ITestOutputHelper output)
        {
            _output = output;
        }

        public void WriteLine(string line)
        {
            _output.WriteLine(line);
        }
    }
}