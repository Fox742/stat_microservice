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
            
            string command = 
                string.Format(
                @"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{0}')
                BEGIN CREATE DATABASE {0}
                END",
                databaseName);

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

        private static List<Dictionary<string, string>> ExecuteReader(SqlCommand command)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                item["key"] = reader["keyEvent"].ToString();
                item["json"] = reader["jsonEvent"].ToString();
                item["dt"] = reader["timeClient"].ToString();
                result.Add(item);
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
            string createCommand =
                string.Format(
                    @"IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') AND type in (N'U'))
            BEGIN
                CREATE TABLE {0}(
	                [id]            [int] IDENTITY(1,1) NOT NULL,
	                [keyEvent]      [nvarchar] (max) NOT NULL,
	                [jsonEvent]     [nvarchar] (max) NULL,
                    [timeServer]    [datetime] NOT NULL,
	                [timeClient]    [datetime] NOT NULL
                ) ON [PRIMARY]
            END", tableName);

            string connectionString = _commonConnectionString + "Initial Catalog="+_databaseName;

            ExecuteScalarNonParameters(connectionString,createCommand);
        }

        public static void WriteStatistics(string key, string eventJson, DateTime clientDT)
        {
            CreateDBIfNotExists(_commonConnectionString, _databaseName);
            string tableName = getTableName(key);
            createTableIfNotExists(tableName);

            string command =
                @"INSERT INTO " + tableName + @" (keyEvent, jsonEvent, timeServer, timeClient) VALUES(@keyEvent, @jsonEvent, @timeServer, @timeClient)";


            string connectionString = _commonConnectionString + "Initial Catalog=" + _databaseName;

            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(command, myConn);
                myCommand.Parameters.AddWithValue("@keyEvent", key);
                myCommand.Parameters.AddWithValue("@jsonEvent", eventJson);
                myCommand.Parameters.AddWithValue("@timeServer", DateTime.Now.ToUniversalTime());
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

            string command = 
                string.Format(
                    @"SELECT
                        keyEvent, jsonEvent, timeClient FROM {0}
                    WHERE (keyEvent = '{1}') 
                        AND (timeClient >= @start)
                        AND (timeClient <= @finish)",
                    tableName,
                    key
                );

            if (start == null)
                start = new DateTime(1754, 1, 1, 0, 0, 0);

            if (finish == null)
                finish = DateTime.MaxValue;

            string connectionString = _commonConnectionString + "Initial Catalog=" + _databaseName;

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>(); 
            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(command, myConn);
                myCommand.Parameters.AddWithValue("@start", ((DateTime)start).ToUniversalTime()); // (Минимальная дата в MS SQL Server)
                myCommand.Parameters.AddWithValue("@finish", ((DateTime)finish).ToUniversalTime());

                result = ExecuteReader(myCommand);
                myConn.Close();
            }

            return result;
        }

        public static void RemoveAll()
        {
            string droppingScript =
                string.Format(
                @"IF EXISTS(SELECT * FROM sys.databases WHERE name = '{0}')
                BEGIN
                    USE master
                    ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                    DROP DATABASE  {0}
                END",
                _databaseName);

            string connectionString = _commonConnectionString + "Initial Catalog=master";

            ExecuteScalarNonParameters(connectionString, droppingScript);
        }
    }
}
