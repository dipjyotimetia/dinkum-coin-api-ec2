using System;
using DinkumCoin.Core.Contracts;
using Moq;
using Xunit;

namespace DinkumCoin.Services.Tests
{
    public class MiningServiceTests
    {
        [Fact]
        public void GivenPrimeNumCalcSuccessful_ThenNewCoinReturned()
        {
            // Arange
            var mathMock = new Mock<IMathService>();
            mathMock.Setup(x => x.IsPrime(It.IsAny<int>())).Returns(true);
            var miningService = new MiningService(mathMock.Object);

            // Act
            var result = miningService.AttemptMineNewCoin();

            // Assert
            Assert.True(result.CoinCreated);
            Assert.NotNull(result.NewCoin);
        }

        [Fact]
        public void GivenPrimeNumCalcUnsuccessful_ThenNoCoinReturned()
        {
            // Arange
            var mathMock = new Mock<IMathService>();
            mathMock.Setup(x => x.IsPrime(It.IsAny<int>())).Returns(false);
            var miningService = new MiningService(mathMock.Object);

            // Act
            var result = miningService.AttemptMineNewCoin();


            // Assert
            Assert.False(result.CoinCreated);
            Assert.Null(result.NewCoin);
        }
    }
}
