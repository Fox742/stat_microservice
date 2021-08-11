using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StatMicroservice
{
    public class DataBasePort
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

            SqlConnection myConn = new SqlConnection(connectionString);
            myConn.Open();
            SqlCommand myCommand = new SqlCommand(command, myConn);
            myCommand.ExecuteNonQuery();
            myConn.Close();
        }
    }
}
