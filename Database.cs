using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace EmployeeManagementSystem
{
    public static class Database
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["EMSConnection"].ConnectionString;

        public static void Initialize()
        {
            try
            {
                string dataDir = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString()
                    ?? AppDomain.CurrentDomain.BaseDirectory;
                Directory.CreateDirectory(dataDir);
                string dbPath = Path.Combine(dataDir, "employee.mdf");
                if (!File.Exists(dbPath))
                {
                    CreateDatabase(dbPath);
                }

                EnsureTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database initialization error: " + ex.Message);
                throw;
            }
        }

        private static void CreateDatabase(string mdfPath)
        {
            string logPath = Path.ChangeExtension(mdfPath, ".ldf");
            string dbName = Path.GetFileNameWithoutExtension(mdfPath);
            string createDb = $"CREATE DATABASE [{dbName}] ON (NAME='{dbName}', FILENAME='{mdfPath}') " +
                $"LOG ON (NAME='{dbName}_log', FILENAME='{logPath}')";

            using (var connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;"))
            {
                connection.Open();
                using (var command = new SqlCommand(createDb, connection))
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
