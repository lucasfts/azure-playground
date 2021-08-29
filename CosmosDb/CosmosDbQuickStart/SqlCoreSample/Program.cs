using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace SqlCoreSample
{
    class Program
    {
        static string EndpointUrl = Environment.GetEnvironmentVariable("CosmosEndpointUrl");
        static string PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
        static string databaseId = "FamilyDatabase";
        static string containerId = "FamilyContainer";

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                await StartDemoAsync();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private static async Task StartDemoAsync()
        {
            var client = new CosmosClient(EndpointUrl, PrimaryKey);
            var service = new CosmosService(client);

            await service.CreateDatabaseAsync(databaseId);
            await service.CreateContainerAsync(containerId);

            Family family = GetAndersenFamily();
            await service.AddItemToContainerAsync(family);

            await service.QueryItemsAsync();

            await service.DeleteDatabaseAndCleanupAsync();
        }

        private static Family GetAndersenFamily()
        {
            return new Family
            {
                Id = "Andersen.1",
                LastName = "Andersen",
                Parents = new Parent[]
                {
                   new Parent { FirstName = "Thomas" },
                   new Parent { FirstName = "Mary Kay" }
                },
                Children = new Child[]
                {
                new Child
                {
                    FirstName = "Henriette Thaulow",
                    Gender = "female",
                    Grade = 5,
                    Pets = new Pet[]
                    {
                        new Pet { GivenName = "Fluffy" }
                    }
                }
                },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = false
            };
        }
    }
}
