using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CosmosDbPerformanceMeasurement
{
    internal class Program
    {
        private static readonly CosmosClient CosmosClient = new CosmosClient("AccountEndpoint=https://cosdb-playground-dev.documents.azure.com:443/;AccountKey=68ZRXHwjR1bKym5whlU4aIWLTF8yf2Uy0oyrYMzmvJcGTNcYCnK4W9PQu9GNMjcIIHvPcdt9OmxbTRnv2T6jgw==;");
        public static async Task Main()
        {
            const string DatabaseId = "TestDb";
            const string ContainerId = "TestContainer";
            const int Throughput = 400;

            Console.WriteLine($"Create database '{DatabaseId}' with throughput {Throughput}");
            await CreateDatabase(CosmosClient, DatabaseId, Throughput);
                
            Console.WriteLine($"Create container '{ContainerId}' with throughput {Throughput}");
            // TODO: uncomment specific indexing policy
            //await CreateContainerWithNoneIndexingPolicy(CosmosClient, DatabaseId, ContainerId, Throughput);
            //await CreateContainerWithAutomaticIndexingPolicy(CosmosClient, DatabaseId, ContainerId, Throughput);
            //await CreateContainerWithPartitionKeyOnlyIndexingPolicy(CosmosClient, DatabaseId, ContainerId, Throughput);
            //await CreateContainerWithAutomaticButExcludedAllIndexingPolicy(CosmosClient, DatabaseId, ContainerId, Throughput);

            Console.WriteLine("Press any key to start write documents");
            Console.ReadLine();

            int numberOfPartitions = 5;
            int numberOfDocumentsToBeInserted = 1000;
            Container container = CosmosClient.GetContainer(DatabaseId, ContainerId);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var writeRusCount = await CreateDocuments(container, numberOfPartitions, numberOfDocumentsToBeInserted);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed write time {stopwatch.Elapsed}. Consumed {writeRusCount} RU's for {numberOfDocumentsToBeInserted} inserted documents distributed in {numberOfPartitions} partitions.");

            long currentIndexTransformationProgress = await CheckIndexTransformationProgress(container);
            Console.WriteLine($"Current index transformation progress: {currentIndexTransformationProgress}");
            while (currentIndexTransformationProgress != 100)
            {
                currentIndexTransformationProgress = await CheckIndexTransformationProgress(container);
                Console.WriteLine($"Current index transformation progress: {currentIndexTransformationProgress}");
            }

            Console.WriteLine("Press any key to start read documents");
            Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();
            var readRusCount = await ReadDocumentsSequentialByPartition(container, numberOfPartitions);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed read time {stopwatch.Elapsed}. Consumed {readRusCount} RU's for {numberOfDocumentsToBeInserted} documents distributed in {numberOfPartitions} partitions.");

            Console.WriteLine($"Delete container {ContainerId}");
            await DeleteContainer(CosmosClient, DatabaseId, ContainerId);
            Console.ReadLine();
        }

        class Order
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("orderNumber")]
            public string OrderNumber { get; set; }

            [JsonProperty("partitionKey")]
            public string PartitionKey { get; set; }
        }

        private static async Task CreateDatabase(CosmosClient cosmosClient, string databaseId, int throughput) => 
            await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId, throughput);

        private static async Task DeleteDatabase(CosmosClient cosmosClient, string databaseId) =>
            await cosmosClient.GetDatabase(databaseId).DeleteAsync();

        private static async Task DeleteContainer(CosmosClient cosmosClient, string databaseId, string containerId) => 
            await cosmosClient.GetContainer(databaseId, containerId).DeleteContainerAsync();

        private static async Task CreateContainerWithAutomaticIndexingPolicy(CosmosClient cosmosClient, string databaseId, string containerId, int? throughput, string partitionKey = "/partitionKey")
        {
            await cosmosClient
                .GetDatabase(databaseId).DefineContainer(containerId, partitionKey)
                .WithIndexingPolicy().WithAutomaticIndexing(true).Attach().CreateAsync(throughput);
        }

        private static async Task CreateContainerWithNoneIndexingPolicy(CosmosClient cosmosClient, string databaseId, string containerId, int? throughput, string partitionKey = "/partitionKey")
        {
            await cosmosClient
                .GetDatabase(databaseId).DefineContainer(containerId, partitionKey).WithIndexingPolicy().WithAutomaticIndexing(false).WithIndexingMode(IndexingMode.None).Attach().CreateAsync(throughput);
        }

        private static async Task CreateContainerWithPartitionKeyOnlyIndexingPolicy(CosmosClient cosmosClient, string databaseId, string containerId, int? throughput, string partitionKey = "/partitionKey")
        {
            await cosmosClient
                .GetDatabase(databaseId)
                .DefineContainer(containerId, partitionKey)
                    .WithIndexingPolicy()
                        .WithExcludedPaths().Path("/*").Attach()
                        .WithIncludedPaths().Path($"{partitionKey}/?")
                        .Attach()
                .Attach()
                .CreateAsync(throughput);
        }

        private static async Task CreateContainerWithAutomaticButExcludedAllIndexingPolicy(CosmosClient cosmosClient, string databaseId, string containerId, int? throughput, string partitionKey = "/partitionKey")
        {
            await cosmosClient
                .GetDatabase(databaseId)
                .DefineContainer(containerId, partitionKey)
                .WithIndexingPolicy()
                .WithExcludedPaths().Path("/*").Attach()
                .Attach()
                .CreateAsync(throughput);
        }

        private static async Task<double> CreateDocuments(Container container, int numberOfPartitions, int numberOfDocumentsToBeInserted)
        {
            double rusCount = 0;
            int documentsCreated = 0;

            for (int i = 1; i <= numberOfPartitions; i++)
            {
                int numberOfDocumentsToInsert =
                    CalculateNumberOfDocumentsToInsert(i, numberOfPartitions, numberOfDocumentsToBeInserted);

                Console.WriteLine($"Insert {numberOfDocumentsToInsert} document(s) into partition {i}");

                for (int j = 1; j <= numberOfDocumentsToInsert; j++)
                {
                    var order = new Order()
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderNumber = i.ToString(),
                        PartitionKey = i.ToString()
                    };

                    Console.WriteLine($"Insert document id '{order.Id}' into partition '{order.PartitionKey}'");
                    ItemResponse<Order> itemResponse = await container.CreateItemAsync(order, new PartitionKey(order.PartitionKey));
                    rusCount += itemResponse.RequestCharge;
                    documentsCreated += 1;
                }

            }

            Console.WriteLine($"Documents created: {documentsCreated}");
            return rusCount;
        }

        private static int CalculateNumberOfDocumentsToInsert(int currentIndex, int numberOfPartitions, int numberOfDocumentsToBeInserted)
        {
            int itemsPerPartition = (numberOfDocumentsToBeInserted / numberOfPartitions);
            int extraItem = currentIndex <= (numberOfDocumentsToBeInserted % numberOfPartitions) ? 1 : 0;
            return itemsPerPartition + extraItem;
        }

        private static async Task<double> ReadDocumentsSequentialByPartition(Container container, int numberOfPartitions)
        {
            double rusCount = 0;
            int documentsRead = 0;
            for (int i = 1; i <= numberOfPartitions; i++)
            {
                var iterator = container.GetItemQueryIterator<Order>("SELECT * FROM c",null, new QueryRequestOptions() {PartitionKey = new PartitionKey(i.ToString())});
                while (iterator.HasMoreResults)
                {
                    FeedResponse<Order> result = await iterator.ReadNextAsync();
                    rusCount += result.RequestCharge;

                    foreach (Order order in result)
                    {
                        Console.WriteLine($"Retrieved document id '{order.Id}' from partition '{i}'");
                        documentsRead += 1;
                    }
                }
            }

            Console.WriteLine($"Documents read: {documentsRead}");
            return rusCount;
        }

        private static async Task<long> CheckIndexTransformationProgress(Container container)
        {
            ContainerResponse containerResponse = await container.ReadContainerAsync(new ContainerRequestOptions { PopulateQuotaInfo = true });
            return long.Parse(containerResponse.Headers["x-ms-documentdb-collection-index-transformation-progress"]);
        }
    }


}