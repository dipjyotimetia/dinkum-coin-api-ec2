using System;
using Xunit;
using Xunit.Abstractions;
using DinkumCoin.Api.PactVerify.Framework;
using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;
using PactNet;
using Microsoft.AspNetCore.Hosting;

namespace DinkumCoin.Api.PactTests
{
    public class PactVerificationTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public PactVerificationTests( ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EnsureDinkumCoinApiHonoursPactWithConsumer()
        {
            //Arrange
            const string baseUrl = "http://localhost:9011";
            var fixture = new TestServerFixture();

            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_output)
                }, Verbose = true
            };

            using (IWebHost webHost = fixture.CreateWebHost(baseUrl))
            {
                webHost.Start();

            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
              //  .ProviderState($"{serviceUri}/provider-states")
                .ServiceProvider("dinkum-coin-api", baseUrl)
                .HonoursPactWith("dinkum-coin-web")
                .PactUri($"pacts/dinkum-coin-web-dinkum-coin-api.json")
                .Verify();
            
            }
        }


        public virtual void Dispose()
        {
            
        }
    }
    



}
