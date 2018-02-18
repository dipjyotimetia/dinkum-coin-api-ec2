using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DinkumCoin.Core.Models;

namespace DinkumCoin.Core.Contracts
{
    public interface IDinkumRepository
    {
        Task<IDictionary<string, string>> GetAllWallets();

        Task<Wallet> GetWallet(Guid walletId);

        Task<Wallet> AddCoinToWallet(Guid walletId, Coin newCoin);

        Task<bool> WriteLedger(LedgerRecord record);

        Task<Wallet> CreateWallet(Wallet newWallet);
    }
}
