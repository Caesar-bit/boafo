using System;
using System.Configuration;
using System.IO;
using Mono.Data.Sqlite;

namespace EmployeeManagementSystem
{
    public static class Database
    {
        private static readonly string dbPath;
        private static readonly string connectionString;

        static Database()
        {
            var conn = ConfigurationManager.ConnectionStrings["EMSConnection"]?.ConnectionString;
            if (!string.IsNullOrEmpty(conn))
            {
                var builder = new SqliteConnectionStringBuilder(conn);
                dbPath = Path.IsPathRooted(builder.DataSource)
                    ? builder.DataSource
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, builder.DataSource);
                connectionString = $"Data Source={dbPath};Version=3;";
            }
            else
            {
                dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employee.db");
                connectionString = $"Data Source={dbPath};Version=3;";
            }
        }

        public static void Initialize()
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    SqliteConnection.CreateFile(dbPath);
                }
                EnsureTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database initialization error: " + ex.Message);
                throw;
            }
        }

        private static void EnsureTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string createEmployeesTable = @"CREATE TABLE IF NOT EXISTS employees (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    employee_id TEXT NOT NULL,
                    full_name TEXT NOT NULL,
                    gender TEXT NOT NULL,
                    contact_number TEXT NOT NULL,
                    position TEXT NOT NULL,
                    image TEXT NOT NULL,
                    salary INTEGER NOT NULL DEFAULT 0,
                    insert_date TEXT NOT NULL,
                    update_date TEXT,
                    delete_date TEXT,
                    status TEXT NOT NULL
                );";

                using (var command = new SqliteCommand(createEmployeesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string createUsersTable = @"CREATE TABLE IF NOT EXISTS users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    date_registered TEXT NOT NULL
                );";

                using (var command = new SqliteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }
    }
}
