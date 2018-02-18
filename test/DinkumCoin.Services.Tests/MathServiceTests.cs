using System;
using Xunit;

namespace DinkumCoin.Services.Tests
{
    public class MathServiceTests
    {

        [Theory]
        [InlineData(1223, true)]
        [InlineData(10159, true)]
        [InlineData(9, false)]
        [InlineData(8000, false)]
        public void IsPrimeMethodTests(int candidate, bool expectedEvaluation) {

            // Arrange
            var mathService = new MathService();

            // Act
            var result = mathService.IsPrime(candidate);


            // Assert
            Assert.Equal(expectedEvaluation, result);
        }
    }
}
