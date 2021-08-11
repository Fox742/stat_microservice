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
            _commonConnectionString = commonConnectionString;
            _databaseName = databaseName;

            string connectionString = _commonConnectionString + "Initial Catalog=master";
            
            string command = "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '" + _databaseName + "')"
            + "BEGIN CREATE DATABASE " + _databaseName + ";"
            + "END";

            ExecuteScalar(connectionString, command);
        }

        private static void ExecuteScalar(string connectionString, string command)
        {
            SqlConnection myConn = new SqlConnection(connectionString);
            myConn.Open();
            SqlCommand myCommand = new SqlCommand(command, myConn);
            myCommand.ExecuteNonQuery();
            myConn.Close();
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

            ExecuteScalar(connectionString,createCommand);
        }

        public static void WriteStatistics(string key, string eventJson, DateTime? clientDT)
        {
            string tableName = getTableName(key);
            createTableIfNotExists(tableName);

            string command = @"INSERT INTO "
                    + tableName + @" (keyEvent, jsonEvent, timeServer";

            if (clientDT != null)
                command += ", timeClient";

            command += ") VALUES ('" + key + "', '" + eventJson + "', getdate()";

            if (clientDT != null)
                command += ", " + clientDT.ToString();

            command += ")";

            string connectionString = _commonConnectionString + "Initial Catalog=" + _databaseName;

            ExecuteScalar(connectionString, command);
        }

    }
}
