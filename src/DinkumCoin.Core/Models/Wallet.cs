using System;
using System.Collections.Generic;

namespace DinkumCoin.Core.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public DateTime? CreationDate { get; set; }
        public string WalletName { get; set; }
        public IList<Coin> Coins { get; set; }
    }
}
