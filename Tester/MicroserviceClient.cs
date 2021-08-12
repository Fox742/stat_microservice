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

            Thread.Sleep(3000);

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

        private void PostEvent(JToken oneEvent)
        {
            var parameters2 = new Dictionary<string, string> {
                { "key", (string)oneEvent["key"] },
                { "eventJson", oneEvent["json"].ToString(Formatting.None) },
                { "clientDT", (string)oneEvent["dt"] }
            };

            var encodedContent2 = new FormUrlEncodedContent(parameters2);

            var response2 = _client.PostAsync("statistics/add", encodedContent2).Result;
        }

        public void SendToServer(List<JToken>objects)
        {
            foreach (JToken oneObj in objects)
            {
                PostEvent(oneObj);
            }
        }

        public JToken GetSorted(int pageSize = -1, int pageNumber = -1)
        {
            string urlQuery = "statistics/get?key=some_key&field=field1";
            
            var response = _client.GetAsync(urlQuery).Result;
            Console.WriteLine(response.ToString());

            var result = response.Content.ReadAsStringAsync().Result;
            var items = JToken.Parse(result);
            foreach (var oneToken in items)
            {
                if (oneToken["json"].Type == JTokenType.String)
                    oneToken["json"] = JToken.Parse(oneToken["json"].Value<string>());
            }
            return items;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
