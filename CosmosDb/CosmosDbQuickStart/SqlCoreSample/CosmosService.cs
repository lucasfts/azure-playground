using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SqlCoreSample
{
    public class CosmosService
    {
        private CosmosClient client;

        public Database Database { get; private set; }
        public Container Container { get; private set; }

        public CosmosService(CosmosClient cosmosClient)
        {
            client = cosmosClient;
        }

        public async Task CreateDatabaseAsync(string databaseId)
        {
            Database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", Database.Id);
        }

        public async Task CreateContainerAsync(string containerId)
        {
            Container = await Database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
            Console.WriteLine("Created Container: {0}\n", Container.Id);
        }

        public async Task AddItemToContainerAsync(Family family)
        {
            try
            {
                var partitionKey = new PartitionKey(family.LastName);
                var familyResponse = await Container.CreateItemAsync(family, partitionKey);
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", familyResponse.Resource.Id, familyResponse.RequestCharge);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Item in database with id: {0} already exists\n", family.Id);
            }
        }

        public async Task DeleteDatabaseAndCleanupAsync()
        {
            var databaseId = Database.Id;
            await Database.DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", databaseId);

            client.Dispose();
        }

        public async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.LastName = 'Andersen'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            var queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = Container.GetItemQueryIterator<Family>(queryDefinition);

            var families = new List<Family>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var family in currentResultSet)
                {
                    families.Add(family);
                    Console.WriteLine("\tRead {0}\n", family);
                }
            }
        }
    }
}
