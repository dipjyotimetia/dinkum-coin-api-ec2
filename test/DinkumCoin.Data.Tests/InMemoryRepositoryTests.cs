using System;
using DinkumCoin.Core.Models;
using DinkumCoin.Data.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace DinkumCoin.Data.Tests
{
    public class InMemoryRepositoryTests : IClassFixture<RepoTestFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly RepoTestFixture _testFixture;

        public InMemoryRepositoryTests(RepoTestFixture testFixture, ITestOutputHelper output)
        {
            _output = output;
            _testFixture = testFixture;
        }



        [Fact]
        public void AddWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();
            var myNewWallet = new Wallet() { Id = Guid.NewGuid(), WalletName = "Test Wallet-"+Guid.NewGuid(), CreationDate = DateTime.Now };

            // Act
            var repoResult = repo.CreateWallet(myNewWallet).Result;

            // Assert 
            Assert.NotNull(repoResult);
            Assert.IsType(typeof(Wallet), repoResult);
        }

        [Fact]
        public void GetAllWalletsTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            // Act
            var repoResult = repo.GetAllWallets().Result;

            // Assert
            Assert.NotEmpty(repoResult);

        }

        [Fact]
        public void GetSpecificWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            var walletId = new Guid("dd9fbf9b-a500-4c00-b00d-069ea4080004");

            // Act
            var repoResult = repo.GetWallet(walletId).Result;

            // Assert 
            Assert.Equal(walletId, repoResult.Id);
            Assert.Equal("Test Wallet", repoResult.WalletName);
        }


        [Fact]
        public void AddCoinToWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            var walletId = new Guid("dd9fbf9b-a500-4c00-b00d-069ea4080004");
            var coin = new Coin() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
            // Act
            var updateResult = repo.AddCoinToWallet(walletId, coin).Result;

            // Assert 
            Assert.NotNull(updateResult);
            Assert.IsType(typeof(Wallet),updateResult);
        }




    }
}
