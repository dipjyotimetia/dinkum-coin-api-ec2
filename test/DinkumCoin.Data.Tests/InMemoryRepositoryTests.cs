using System;
using System.Threading.Tasks;
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
        public async Task AddWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();
            var myNewWallet = new Wallet() { Id = Guid.NewGuid(), WalletName = "Test Wallet-"+Guid.NewGuid(), CreationDate = DateTime.Now };

            // Act
            var repoResult = await repo.CreateWallet(myNewWallet);

            // Assert 
            Assert.NotNull(repoResult);
            Assert.IsType(typeof(Wallet), repoResult);
        }

        [Fact]
        public async Task GetAllWalletsTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            // Act
            var repoResult = await repo.GetAllWallets();

            // Assert
            Assert.NotEmpty(repoResult);

        }

        [Fact]
        public async Task GetSpecificWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            var walletId = new Guid("dd9fbf9b-a500-4c00-b00d-069ea4080004");

            // Act
            var repoResult = await repo.GetWallet(walletId);

            // Assert 
            Assert.Equal(walletId, repoResult.Id);
            Assert.Equal("Test Wallet", repoResult.WalletName);
        }


        [Fact]
        public async Task AddCoinToWalletTest()
        {
            // Arrange
            var repo = new InMemoryRepository();

            var walletId = new Guid("dd9fbf9b-a500-4c00-b00d-069ea4080004");
            var coin = new Coin() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
            // Act
            var updateResult = await repo.AddCoinToWallet(walletId, coin);

            // Assert 
            Assert.NotNull(updateResult);
            Assert.IsType(typeof(Wallet),updateResult);
        }




    }
}
