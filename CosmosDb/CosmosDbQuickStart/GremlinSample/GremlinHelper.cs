using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace GremlinSample
{
    public static class GremlinHelper
    {
        private static string Host => Environment.GetEnvironmentVariable("GremlinHost");
        private static string PrimaryKey => Environment.GetEnvironmentVariable("GremlinPrimaryKey");
        private static string Database => "graphdb";
        private static string Container => "Persons";
        private static bool EnableSSL => true;
        private static int Port => 443;

        public static GremlinClient GetGremlinClient()
        {
            var containerLink = "/dbs/" + Database + "/colls/" + Container;
            Console.WriteLine($"Connecting to: host: {Host}, port: {Port}, container: {containerLink}, ssl: {EnableSSL}");

            var server = new GremlinServer(Host, Port, EnableSSL, containerLink, PrimaryKey);

            var connectionPoolSettings = new ConnectionPoolSettings
            {
                MaxInProcessPerConnection = 10,
                PoolSize = 30,
                ReconnectionAttempts = 3,
                ReconnectionBaseDelay = TimeSpan.FromMilliseconds(500)
            };

            var webSocketConfiguration = new Action<ClientWebSocketOptions>(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            });

            var fraphSON2MimeType = "application/vnd.gremlin-v2.0+json";

            var client = new GremlinClient(
                server,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                fraphSON2MimeType,
                connectionPoolSettings,
                webSocketConfiguration);
            return client;
        }

        public static Dictionary<string, string> Queries = new Dictionary<string, string>
        {
            { "Cleanup",        "g.V().drop()" },
            { "AddVertex 1",    "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44).property('pk', 'pk')" },
            { "AddVertex 2",    "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39).property('pk', 'pk')" },
            { "AddVertex 3",    "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller').property('pk', 'pk')" },
            { "AddVertex 4",    "g.addV('person').property('id', 'robin').property('firstName', 'Robin').property('lastName', 'Wakefield').property('pk', 'pk')" },
            { "AddEdge 1",      "g.V('thomas').addE('knows').to(g.V('mary'))" },
            { "AddEdge 2",      "g.V('thomas').addE('knows').to(g.V('ben'))" },
            { "AddEdge 3",      "g.V('ben').addE('knows').to(g.V('robin'))" },
            { "UpdateVertex",   "g.V('thomas').property('age', 44)" },
            { "CountVertices",  "g.V().count()" },
            { "Filter Range",   "g.V().hasLabel('person').has('age', gt(40))" },
            { "Project",        "g.V().hasLabel('person').values('firstName')" },
            { "Sort",           "g.V().hasLabel('person').order().by('firstName', decr)" },
            { "Traverse",       "g.V('thomas').out('knows').hasLabel('person')" },
            { "Traverse 2x",    "g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')" },
            { "Loop",           "g.V('thomas').repeat(out()).until(has('id', 'robin')).path()" },
            { "DropEdge",       "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()" },
            { "CountEdges",     "g.E().count()" },
            { "DropVertex",     "g.V('thomas').drop()" },
        };
    }
}
