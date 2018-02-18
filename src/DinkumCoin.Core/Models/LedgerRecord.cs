using System;
namespace DinkumCoin.Core.Models
{
    public class LedgerRecord
    {
        public Guid Id { get; set; }
        public DateTime TxnTime { get; set; }
        public TxnType TxnType { get; set; }
        public Guid SourceWallet { get; set; }
        public Guid DestinationWallet { get; set; }
        public Guid Coin { get; set; }
    }
}
