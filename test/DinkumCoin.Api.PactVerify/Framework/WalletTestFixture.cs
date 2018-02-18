using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace DinkumCoin.Api.PactVerify
{
    public class WalletTestFixture : IDisposable
    {
    //    private AmazonDynamoDBClient _client;
    //    private Document _itemUnderTest;

        public WalletTestFixture()
        {
            //_client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.APSoutheast2);

            //var table = Table.LoadTable(_client, "Wallet");
            //var search = table.Scan(new ScanFilter());

            //var jsonText = "{\"Coins\":[{\"CreationDate\":\"2017-12-26T14:29:32.387Z\",\"Id\":\"a3e009a2-eba3-4e48-95b7-0eaddbfd7916\"}],\"CreationDate\":\"2017-12-26T14:28:59.096Z\",\"Id\":\"dd9fbf9b-a500-4c00-b00d-069ea4080004\",\"WalletName\":\"Test Wallet\"}";
            //_itemUnderTest = Document.FromJson(jsonText);

            //var result = table.PutItemAsync(_itemUnderTest).Result;

        }

        public void Dispose()
        {
            //var table = Table.LoadTable(_client, "Wallet");

            //var result = table.DeleteItemAsync(_itemUnderTest).Result;
            //_client.Dispose();
        }
    }
}
