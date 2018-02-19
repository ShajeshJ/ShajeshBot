using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsDB.Models
{
    [DynamoDBTable("MentionRoles")]
    public class MentionRole : IAwsDbItem
    {
        public MentionRole() { }

        public const string IDCOLNAME = "Id";

        [DynamoDBHashKey]
        [DynamoDBProperty(IDCOLNAME)]
        public ulong RoleId { get; set; }

        [DynamoDBProperty("Name")]
        public string Name { get; set; }

        public string GetIdFieldName()
        {
            return IDCOLNAME;
        }

        public static async Task CreateTable(AmazonDynamoDBClient client)
        {
            // Build a 'CreateTableRequest' for the new table
            CreateTableRequest createRequest = new CreateTableRequest
            {
                TableName = "MentionRoles",
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                }
            };

            // Provisioned-throughput settings are required even though
            // the local test version of DynamoDB ignores them
            createRequest.ProvisionedThroughput = new ProvisionedThroughput(1, 1);

            // Using the DynamoDB client, make a synchronous CreateTable request
            CreateTableResponse createResponse;
            try
            {
                createResponse = await client.CreateTableAsync(createRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Error: failed to create the new table; " + ex.Message);
                return;
            }
        }
    }
}
