using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Tester
{
    class MicroserviceClient: IDisposable
    {
        private string _connectionString;
        private string _tableName;
        private HttpClient _client;

        public MicroserviceClient()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            _connectionString = config["ConnectionStrings:CommonConnectionString"];
            _tableName = config["ConnectionStrings:DataBaseName"];

            _client = new HttpClient();
            _client.BaseAddress = new Uri(config["MicroserviceBaseUrl"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }

        public bool DropDatabase()
        {
            _client.PostAsync("statistics/clear", null);

            Thread.Sleep(5000);

            string cmd = string.Format(@"SELECT* FROM sys.databases WHERE name = '{0}'", _tableName);
            
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))

            {
                using (SqlCommand com = new SqlCommand(cmd, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        return !reader.HasRows;
                    }
                }
            }
        }

        private void PostEvent(JObject oneEvent)
        {
            var parameters2 = new Dictionary<string, string> {
                { "key", (string)oneEvent["key"] },
                { "eventJson", oneEvent["json"].ToString(Formatting.None) },
                { "clientDT", (string)oneEvent["dt"] }
            };

            var encodedContent2 = new FormUrlEncodedContent(parameters2);

            var response2 = _client.PostAsync("statistics/add", encodedContent2).Result;
        }

        public void SendToServer(List<JObject>objects)
        {
            foreach (JObject oneObj in objects)
            {
                PostEvent(oneObj);
            }
        }

        public void GetSorted()
        {
            var response4 = _client.GetAsync("statistics/get?key=some_key&field=field1").Result;
            Console.WriteLine(response4.ToString());

            var result = response4.Content.ReadAsStringAsync().Result;

            var some = JToken.Parse(result);
            foreach (var oneToken in some)
            {
                JToken jToken;
                if (((JObject)oneToken).TryGetValue("eventJson", out jToken) && jToken.Type == JTokenType.String)
                {
                    oneToken["eventJson"] = JToken.Parse(jToken.Value<string>());
                }

            }

            Console.WriteLine(some.ToString(Newtonsoft.Json.Formatting.Indented));

        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
