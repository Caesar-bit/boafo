using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    public static class Database
    {
        private const string DbName = "EMS";
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["EMSConnection"].ConnectionString;

        public static void Initialize()
        {
            try
            {
                EnsureDatabase();
                EnsureTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database initialization error: " + ex.Message);
                throw;
            }
        }

        private static void EnsureDatabase()
        {
            using (var connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;"))
            {
                connection.Open();
                string sql = $"IF DB_ID('{DbName}') IS NULL CREATE DATABASE [{DbName}]";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string createEmployeesTable = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='employees' AND xtype='U')
                    CREATE TABLE employees (
                        id INT IDENTITY(1,1) PRIMARY KEY,
                        employee_id NVARCHAR(50) NOT NULL,
                        full_name NVARCHAR(100) NOT NULL,
                        gender NVARCHAR(10) NOT NULL,
                        contact_number NVARCHAR(20) NOT NULL,
                        position NVARCHAR(50) NOT NULL,
                        image NVARCHAR(255) NOT NULL,
                        salary INT NOT NULL DEFAULT 0,
                        insert_date DATETIME NOT NULL,
                        update_date DATETIME NULL,
                        delete_date DATETIME NULL,
                        status NVARCHAR(20) NOT NULL
                    );";

                using (var command = new SqlCommand(createEmployeesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
