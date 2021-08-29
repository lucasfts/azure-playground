using Cassandra;
using Cassandra.Mapping;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CassandraSample
{
    class Program
    {
        static string USERNAME = Environment.GetEnvironmentVariable("CASSANDRAUSERNAME");
        static string PASSWORD = Environment.GetEnvironmentVariable("CASSANDRAPASSWORD");
        static string CASSANDRACONTACTPOINT = Environment.GetEnvironmentVariable("CASSANDRACONTACTPOINT");
        static string keySpace = "uprofile";
        static int CASSANDRAPORT = 10350;

        static ICluster cluster;
        static ISession session;
        static IMapper mapper;

        static void Main(string[] args)
        {
            ProcessAsync().Wait();

            Console.WriteLine("Done! You can go look at your data in Cosmos DB. Don't forget to clean up Azure resources!");
            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }

        private static async Task ProcessAsync()
        {
            cluster = await GetCluster();
            session = await cluster.ConnectAsync();

            await CreateKeySpace();
            await CreateUserTable();

            session = await cluster.ConnectAsync(keySpace);
            mapper = new Mapper(session);

            await InsertUsers();
            await ListUsers();

            var userId = 3;
            await ListUserById(userId);
        }

        private static async Task ListUserById(int userId)
        {
            Console.WriteLine($"Getting by id {userId}");
            Console.WriteLine("-------------------------------");
            User user = await mapper.FirstOrDefaultAsync<User>("Select * from user where user_id = ?", userId);
            Console.WriteLine(user);
        }

        private static async Task ListUsers()
        {
            Console.WriteLine("Select ALL");
            Console.WriteLine("-------------------------------");
            var users = await mapper.FetchAsync<User>("Select * from user");
            foreach (User user in users)
                Console.WriteLine(user);
        }

        private static async Task InsertUsers()
        {
            await mapper.InsertAsync(new User(1, "LyubovK", "Dubai"));
            await mapper.InsertAsync(new User(2, "JiriK", "Toronto"));
            await mapper.InsertAsync(new User(3, "IvanH", "Mumbai"));
            await mapper.InsertAsync(new User(4, "LiliyaB", "Seattle"));
            await mapper.InsertAsync(new User(5, "JindrichH", "Buenos Aires"));
            Console.WriteLine("Inserted data into user table");
        }

        private static async Task CreateUserTable()
        {
            await session.ExecuteAsync(new SimpleStatement($"CREATE TABLE IF NOT EXISTS {keySpace}.user (user_id int PRIMARY KEY, user_name text, user_bcity text)"));
            Console.WriteLine(String.Format("created table user"));
        }

        private static async Task CreateKeySpace()
        {
            await session.ExecuteAsync(new SimpleStatement($"DROP KEYSPACE IF EXISTS {keySpace}"));
            await session.ExecuteAsync(new SimpleStatement($"CREATE KEYSPACE {keySpace} WITH REPLICATION = {{ 'class': 'NetworkTopologyStrategy', 'datacenter1' : 1 }};"));
            Console.WriteLine($"created keyspace {keySpace}");
        }

        private static async Task<ICluster> GetCluster()
        {
            var options = new SSLOptions(System.Security.Authentication.SslProtocols.Tls12, true, ValidateServerCertificate);
            options.SetHostNameResolver((ipAddress) => CASSANDRACONTACTPOINT);

            return cluster = Cluster
                .Builder()
                .WithCredentials(USERNAME, PASSWORD)
                .WithPort(CASSANDRAPORT)
                .AddContactPoint(CASSANDRACONTACTPOINT)
                .WithSSL(options)
                .Build();
        }

        private static bool ValidateServerCertificate(
            object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }
    }
}
