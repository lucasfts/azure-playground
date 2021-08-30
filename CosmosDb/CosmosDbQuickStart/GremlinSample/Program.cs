using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GremlinSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = GremlinHelper.GetGremlinClient();

            ExecuteQueries(client);

            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        private static void ExecuteQueries(GremlinClient client)
        {
            using (client)
            {
                foreach (var query in GremlinHelper.Queries)
                {
                    Console.WriteLine(String.Format("Running this query: {0}: {1}", query.Key, query.Value));

                    try
                    {
                        var resultSet = client.SubmitAsync<dynamic>(query.Value).Result;
                        if (resultSet.Count > 0)
                        {
                            Console.WriteLine("\tResult:");
                            foreach (var result in resultSet)
                            {
                                var output = JsonConvert.SerializeObject(result);
                                Console.WriteLine($"\t{output}");
                            }
                            Console.WriteLine();
                        }

                        PrintStatusAttributes(resultSet.StatusAttributes);
                        Console.WriteLine();
                    }
                    catch (ResponseException e)
                    {
                        PrintStatusAttributes(e.StatusAttributes);
                    }
                }
            }
        }

        private static void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes)
        {
            Console.WriteLine($"\tStatusAttributes:");
            Console.WriteLine($"\t[\"x-ms-status-code\"] : { GetValueAsString(attributes, "x-ms-status-code")}");
            Console.WriteLine($"\t[\"x-ms-total-server-time-ms\"] : { GetValueAsString(attributes, "x-ms-total-server-time-ms")}");
            Console.WriteLine($"\t[\"x-ms-total-request-charge\"] : { GetValueAsString(attributes, "x-ms-total-request-charge")}");
        }

        public static string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            return JsonConvert.SerializeObject(GetValueOrDefault(dictionary, key));
        }

        public static object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }
    }
}
