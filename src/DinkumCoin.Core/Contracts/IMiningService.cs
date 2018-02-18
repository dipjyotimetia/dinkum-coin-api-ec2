using DinkumCoin.Core.Models;

namespace DinkumCoin.Core.Contracts
{
    public interface IMiningService
    {
        MiningResult AttemptMineNewCoin();
    }
}
