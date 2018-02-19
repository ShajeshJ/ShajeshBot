using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AwsDB.Models;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon;
using Amazon.DynamoDBv2.Model;
using System.IO;

namespace AwsDB
{
    public class AwsDbContext
    {
        private AmazonDynamoDBClient _client;
        private DynamoDBContext _context;

        public AwsDbContext()
        {
            // First, set up a DynamoDB client for DynamoDB Local
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.RegionEndpoint = RegionEndpoint.USEast2;
            //config.ServiceURL = "http://localhost:8000";
            try
            {
                _client = new AmazonDynamoDBClient(config);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Error: failed to create a DynamoDB client; " + ex.Message);
                throw;
            }

            _context = new DynamoDBContext(_client);
        }

        public async Task Upsert<T>(T item) where T : IAwsDbItem
        {
            await _context.SaveAsync(item);
        }

        public async Task<T> Get<T>(T key) where T : IAwsDbItem
        {
            return await _context.LoadAsync(key);
        }

        public async Task<bool> Exists<T>(AttributeValue key) where T : IAwsDbItem, new()
        {
            var filter = new QueryFilter();
            filter.AddCondition(new T().GetIdFieldName(), ScanOperator.Equal, new List<AttributeValue>() { key });

            var iterator = _context.FromQueryAsync<T>(new QueryOperationConfig() { Filter = filter });

            var results = await iterator.GetRemainingAsync();
            return results.Count > 0;
        }

        public async Task<List<T>> GetAll<T>() where T : IAwsDbItem
        {
            var iterator = _context.FromScanAsync<T>(new ScanOperationConfig());
            return await iterator.GetRemainingAsync();
        }

        public async Task Delete<T>(T dbRoleObj) where T : IAwsDbItem
        {
            await _context.DeleteAsync(dbRoleObj);
        }
    }
}
