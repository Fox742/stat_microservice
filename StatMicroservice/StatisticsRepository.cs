using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StatMicroservice
{
    public class StatisticsRepository
    {
        private static string _commonConnectionString;
        private static string _databaseName;
        public static void CreateDBIfNotExists(string commonConnectionString, string databaseName)
        {

            string connectionString = commonConnectionString + "Initial Catalog=master";
            
            string command = "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '" + databaseName + "')"
            + "BEGIN CREATE DATABASE " + databaseName + ";"
            + "END";

            ExecuteScalarNonParameters(connectionString, command);

            _commonConnectionString = commonConnectionString;
            _databaseName = databaseName;
        }

        private static void ExecuteScalarNonParameters(string connectionString, string command)
        {
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(command, myConn);
                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
        }

        private static IEnumerable<Dictionary<string, string>> ExecuteReader(string connectionString, string command)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand(command, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        Dictionary<string, string> item = new Dictionary<string, string>();
                        item["key"] = reader["keyEvent"].ToString();
                        item["eventJson"] = reader["jsonEvent"].ToString();
                        item["ClientDT"] = reader["timeClient"].ToString();
                        result.Add(item);
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        private static string getTableName(string key)
        {
            return "letter_" + getTablePostfix(key);
        }

        private static string getTablePostfix(string key)
        {
            string pattern = @"^[a-zA-Z]+$";
            Regex regex = new Regex(pattern);
            char firstLetter = key[0];

            if (regex.IsMatch(firstLetter.ToString()))
            {
                return firstLetter.ToString().ToLower();
            }

            return "special";
        }

        private static void createTableIfNotExists(string tableName)
        {
            string createCommand = @"IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'"+ tableName + @"') AND type in (N'U'))
            BEGIN
                CREATE TABLE " + tableName + @"(
	                [id]            [int] IDENTITY(1,1) NOT NULL,
	                [keyEvent]           [nvarchar] (max) NOT NULL,
	                [jsonEvent]     [nvarchar] (max) NULL,
                    [timeServer]    [datetime] NOT NULL,
	                [timeClient]    [datetime] NULL
                ) ON [PRIMARY]
            END";

            string connectionString = _commonConnectionString + "Initial Catalog="+_databaseName;

            ExecuteScalarNonParameters(connectionString,createCommand);
        }

        public static void WriteStatistics(string key, string eventJson, DateTime? clientDT)
        {
            CreateDBIfNotExists(_commonConnectionString, _databaseName);
            string tableName = getTableName(key);
            createTableIfNotExists(tableName);

            string command =
                @"INSERT INTO " + tableName + @" (keyEvent, jsonEvent, timeServer, timeClient) VALUES(@keyEvent, @jsonEvent, @timeServer, @timeClient)";


            string connectionString = _commonConnectionString + "Initial Catalog=" + _databaseName;

            string clientTime = "null";
            if (clientDT!=null)
            {
                clientTime = "'"+clientDT.ToString()+"'";
            }
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(command, myConn);
                myCommand.Parameters.AddWithValue("@keyEvent", key);
                myCommand.Parameters.AddWithValue("@jsonEvent", eventJson);
                myCommand.Parameters.AddWithValue("@timeServer", DateTime.Now.ToUniversalTime());
                if (clientDT == null)
                    myCommand.Parameters.AddWithValue("@timeClient",  DBNull.Value);
                else
                    myCommand.Parameters.AddWithValue("@timeClient", ((DateTime)clientDT).ToUniversalTime());

                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
        }

        public static IEnumerable<Dictionary<string, string>> ReadStatistics(string key, DateTime ? start, DateTime ? finish)
        {
            CreateDBIfNotExists(_commonConnectionString, _databaseName);
            string tableName = getTableName(key);
            createTableIfNotExists(tableName);

            string command = @"SELECT keyEvent, jsonEvent, timeClient FROM " + tableName +
                @" WHERE (keyEvent = '" + key + "')";

            if (start!=null)
            {
                command += " AND (timeClient >= '" + start.ToString() + @"')";
            }

            if (finish != null)
            {
                command += " AND (timeClient <= '" + finish.ToString() + @"')";
            }

            string connectionString = _commonConnectionString + "Initial Catalog=" + _databaseName;

            return ExecuteReader(connectionString, command);
        }
    }
}
