using Microsoft.Extensions.Configuration;
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

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
