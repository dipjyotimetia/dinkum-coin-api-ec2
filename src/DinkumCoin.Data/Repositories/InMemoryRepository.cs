using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkumCoin.Core.Contracts;
using DinkumCoin.Core.Models;

namespace DinkumCoin.Data.Repositories

{
    public class InMemoryRepository : IDinkumRepository
    {

        List<Wallet> AllWallets;

        public InMemoryRepository()
        {
            AllWallets = new List<Wallet>();
            Seed();

        }

        private void Seed(){
            var coins = new List<Coin>();
            coins.Add(new Coin
            {
                CreationDate = DateTime.Parse("2017-12-26T14:29:32.387Z"),
                Id = new Guid("999009a2-eba3-4e48-95b7-0eaddbfd7999")
            });
        
            var wallet = new Wallet { 
                Id = new Guid("dd9fbf9b-a500-4c00-b00d-069ea4080004"),
                WalletName="Test Wallet",
                CreationDate=DateTime.Parse("2017-12-26T14:28:59.096Z"),
                
                Coins=coins
            };
            AllWallets.Add(wallet);
        }

        public async Task<Wallet> AddCoinToWallet(Guid walletId, Coin newCoin)
        {
            return await Task.Run(() => {             
                var wallet = AllWallets.Find(x => x.Id == walletId);
                wallet.Coins.Add(newCoin);
                return wallet;
            });
        }

        public async Task<Wallet> CreateWallet(Wallet newWallet)
        {
            return await Task.Run(() => {     
                 AllWallets.Add(newWallet);
                return newWallet;
            });
        }

        public async Task<IDictionary<string, string>> GetAllWallets()
        {
            return await Task.Run(() => {    
                return AllWallets.ToDictionary(x => x.Id.ToString(), y => y.WalletName);
            });
        }

        public async Task<Wallet> GetWallet(Guid walletId)
        {
            return await Task.Run(() => {   
            var wallet = AllWallets.Find(x => x.Id == walletId);
            return wallet;
            });
        }

        public async Task<bool> WriteLedger(LedgerRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
