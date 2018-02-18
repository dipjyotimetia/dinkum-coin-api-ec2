using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using DinkumCoin.Core.Contracts;
using DinkumCoin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DinkumCoin.Data.Repositories
{
    public class DynamoRepository : IDinkumRepository
    {
        private AmazonDynamoDBClient _client;
        public DynamoRepository()
        {
            _client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.APSoutheast2);
        }

        public async Task<Wallet> AddCoinToWallet(Guid walletId, Coin newCoin)
        {

            var dynamoCoin = new Document();
            dynamoCoin["Id"] = newCoin.Id;
            dynamoCoin["CreationDate"] = newCoin.CreationDate;

            try
            {
                Table walletTable = Table.LoadTable(_client, "Wallet");

                var wallet = await walletTable.GetItemAsync(walletId);
               
                if (!wallet.ContainsKey("Coins"))
                    wallet.Add("Coins", new DynamoDBList());

                var coins = wallet["Coins"].AsDynamoDBList();
                coins.Add(dynamoCoin);

                var response = await walletTable.PutItemAsync(wallet);

                return SerialiseDocToWallet(wallet);
            }

            catch (AmazonDynamoDBException exception)
            {
                throw new Exception($"Exception while updating record in DynamoDb table: {exception.Message}");

            }
        }

        public async Task<IDictionary<string,string>> GetAllWallets()
        {

            var wallets = new Dictionary<string, string>();
            try
            {

                Table walletTable = Table.LoadTable(_client, "Wallet");

                ScanFilter scanFilter = new ScanFilter();
                Search getAllItems = walletTable.Scan(scanFilter);
                var allItems = await getAllItems.GetRemainingAsync();

                return allItems.ToDictionary(x => x["Id"].AsString(), y=> y["WalletName"].AsString());

            }
            catch (AmazonDynamoDBException exception)
            {
                throw new Exception($"Exception while scanning DynamoDb table: {exception.Message}" );
            }
        }

        public async Task<Wallet> GetWallet(Guid walletId)
        {
            
            try
            {
                Table walletTable = Table.LoadTable(_client, "Wallet");

                var response = await walletTable.GetItemAsync(walletId);

                return SerialiseDocToWallet(response);

            }
            catch (AmazonDynamoDBException exception)
            {
                throw new Exception($"Exception while querying DynamoDb table: {exception.Message}");
            }
        }

        public async Task<Wallet> CreateWallet(Wallet newWallet)
        {
            Table walletTable = Table.LoadTable(_client, "Wallet");
            try
            {
                var walletDoc = new Document();
                walletDoc["Id"] = newWallet.Id;
                walletDoc["WalletName"] = newWallet.WalletName;
                walletDoc["CreationDate"] = newWallet.CreationDate;

                var response = await walletTable.PutItemAsync(walletDoc);

                return SerialiseDocToWallet(walletDoc);
            }
            catch (AmazonDynamoDBException exception)
            {
                throw new Exception($"Exception while adding to DynamoDb table: {exception.Message}");
            }
        }



        public async Task<bool> WriteLedger(LedgerRecord record)
        {
            throw new NotImplementedException();
        }


        private Wallet SerialiseDocToWallet(Document doc) {
            var wallet = new Wallet();

            wallet.Id = doc["Id"].AsGuid();
            wallet.WalletName = doc["WalletName"].AsString();

            if (doc.ContainsKey("CreationDate"))
            {
                wallet.CreationDate = doc["CreationDate"].AsDateTime();
            }
            if (doc.ContainsKey("Coins"))
            {
                wallet.Coins = new List<Coin>();
                var coins = doc["Coins"].AsListOfDocument();
                foreach (var coin in coins)
                {
                    wallet.Coins.Add(new Coin()
                    {
                        Id = coin["Id"].AsGuid(),
                        CreationDate = coin["CreationDate"].AsDateTime()
                    });
                }
            }
            return wallet;


        }



    }
}
