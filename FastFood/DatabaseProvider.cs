using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Npgsql;
using Microsoft.Win32;

namespace FastFood
{
    /// <summary>
    /// Database provider interface for supporting multiple database types
    /// </summary>
    public interface IDatabaseProvider
    {
        DbConnection CreateConnection();
        DbCommand CreateCommand(string sql, DbConnection connection);
        DbParameter CreateParameter(string name, object value);
        string ConnectionString { get; set; }
        DatabaseType ProviderType { get; }
    }

    public enum DatabaseType
    {
        SqlServer,
        PostgreSql
    }

    /// <summary>
    /// SQL Server Provider - original implementation
    /// </summary>
    public class SqlServerProvider : IDatabaseProvider
    {
        public string ConnectionString { get; set; }
        public DatabaseType ProviderType => DatabaseType.SqlServer;

        public DbConnection CreateConnection() => new System.Data.SqlClient.SqlConnection(ConnectionString);
        
        public DbCommand CreateCommand(string sql, DbConnection connection) => 
            new System.Data.SqlClient.SqlCommand(sql, (System.Data.SqlClient.SqlConnection)connection);
        
        public DbParameter CreateParameter(string name, object value)
        {
            var param = new System.Data.SqlClient.SqlParameter(name, value ?? DBNull.Value);
            return param;
        }
    }

    /// <summary>
    /// PostgreSQL Provider - new implementation for migration
    /// </summary>
    public class PostgreSqlProvider : IDatabaseProvider
    {
        public string ConnectionString { get; set; }
        public DatabaseType ProviderType => DatabaseType.PostgreSql;

        public DbConnection CreateConnection() => new NpgsqlConnection(ConnectionString);
        
        public DbCommand CreateCommand(string sql, DbConnection connection)
        {
            // Convert SQL Server syntax to PostgreSQL
            string pgSql = SqlNormalizer.Normalize(sql, DatabaseType.PostgreSql);
            return new NpgsqlCommand(pgSql, (NpgsqlConnection)connection);
        }
        
        public DbParameter CreateParameter(string name, object value)
        {
            // PostgreSQL uses different parameter prefix
            string pgName = name.StartsWith("@") ? name : "@" + name;
            var param = new NpgsqlParameter(pgName, value ?? DBNull.Value);
            
            // Handle type mappings
            if (value is DateTime)
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Timestamp;
            else if (value is bool)
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Boolean;
            else if (value is Guid)
                param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
                
            return param;
        }
    }

    /// <summary>
    /// SQL Syntax Normalizer - converts SQL Server syntax to PostgreSQL
    /// </summary>
    public static class SqlNormalizer
    {
        public static string Normalize(string sql, DatabaseType targetDb)
        {
            if (string.IsNullOrWhiteSpace(sql) || targetDb == DatabaseType.SqlServer)
                return sql;

            string result = sql;

            // 1. GETDATE() -> NOW() or CURRENT_TIMESTAMP
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bGETDATE\s*\(\s*\)", "NOW()", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 2. ISNULL(a,b) -> COALESCE(a,b)
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bISNULL\s*\(", "COALESCE(", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 3. LEN(x) -> LENGTH(x)
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bLEN\s*\(", "LENGTH(", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 4. TOP N -> LIMIT N (handled separately for complex queries)
            result = ConvertTopToLimit(result);

            // 5. Square brackets [column] -> "column" or just column
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\[([^\]]+)\]", "\"$1\"");

            // 6. NVARCHAR(MAX) -> TEXT
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bNVARCHAR\s*\(\s*MAX\s*\)", "TEXT", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 7. BIT -> BOOLEAN
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bBIT\b", "BOOLEAN", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // 8. Remove N'string' prefix (Unicode strings)
            result = System.Text.RegularExpressions.Regex.Replace(
                result, @"\bN'", "'");

            // 9. CONVERT/CAST syntax adjustments
            result = ConvertCastSyntax(result);

            return result;
        }

        private static string ConvertTopToLimit(string sql)
        {
            // Pattern: SELECT TOP (N) or SELECT TOP N
            var match = System.Text.RegularExpressions.Regex.Match(
                sql, @"\bSELECT\s+TOP\s*\(?\s*(\d+)\s*\)?", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string limit = match.Groups[1].Value;
                // Remove TOP clause
                sql = System.Text.RegularExpressions.Regex.Replace(
                    sql, @"\bTOP\s*\(?\s*\d+\s*\)?", "", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Add LIMIT at end (before any trailing semicolon)
                sql = sql.TrimEnd().TrimEnd(';') + " LIMIT " + limit;
            }

            return sql;
        }

        private static string ConvertCastSyntax(string sql)
        {
            // CONVERT(type, value) -> CAST(value AS type) - simplified version
            // This handles basic cases; complex conversions may need manual review
            return System.Text.RegularExpressions.Regex.Replace(
                sql, 
                @"\bCONVERT\s*\(\s*(\w+)\s*,\s*([^,)]+)\s*\)", 
                "CAST($2 AS $1)", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }

    /// <summary>
    /// Updated DBObject class with provider abstraction
    /// </summary>
    public class DBObjectNew
    {
        private string m_table;
        private string m_selectString = String.Empty;
        private static IDatabaseProvider _provider;
        private static readonly object _lock = new object();

        public string Table
        {
            get { return m_table; }
            set { m_table = value; }
        }

        protected string SelectString
        {
            get { return m_selectString; }
            set { m_selectString = value; }
        }

        /// <summary>
        /// Initialize database provider based on configuration
        /// </summary>
        public static void Initialize(DatabaseType dbType, string connectionString)
        {
            lock (_lock)
            {
                switch (dbType)
                {
                    case DatabaseType.PostgreSql:
                        _provider = new PostgreSqlProvider { ConnectionString = connectionString };
                        break;
                    case DatabaseType.SqlServer:
                    default:
                        _provider = new SqlServerProvider { ConnectionString = connectionString };
                        break;
                }
            }
        }

        /// <summary>
        /// Get provider from registry settings (backward compatible)
        /// </summary>
        private static IDatabaseProvider GetProvider()
        {
            if (_provider == null)
            {
                lock (_lock)
                {
                    if (_provider == null)
                    {
                        // Read from registry for backward compatibility
                        RegistryKey rk = Registry.CurrentUser.OpenSubKey("Connection");
                        if (rk == null)
                        {
                            rk = Registry.CurrentUser.CreateSubKey("Connection");
                            rk.SetValue("ConnectionString", @"Data Source=localhost\SQLEXPRESS;Initial Catalog=FastFood;Integrated Security=True");
                            rk.SetValue("DatabaseType", "SqlServer");
                        }

                        string connStr = (string)rk.GetValue("ConnectionString");
                        string dbTypeStr = (string)rk.GetValue("DatabaseType") ?? "SqlServer";
                        
                        DatabaseType dbType = dbTypeStr.Equals("PostgreSql", StringComparison.OrdinalIgnoreCase) 
                            ? DatabaseType.PostgreSql 
                            : DatabaseType.SqlServer;

                        Initialize(dbType, connStr);
                    }
                }
            }
            return _provider;
        }

        public DataTable Select(string filter, string order, int top)
        {
            var provider = GetProvider();
            string sql = m_selectString;

            if (top > 0)
            {
                if (provider.ProviderType == DatabaseType.PostgreSql)
                {
                    // PostgreSQL: LIMIT at end
                    sql = sql + (string.IsNullOrEmpty(filter) ? "" : " WHERE " + filter);
                    if (!string.IsNullOrEmpty(order)) sql += " ORDER BY " + order;
                    sql += " LIMIT " + top;
                }
                else
                {
                    // SQL Server: TOP after SELECT
                    if (!sql.Contains(" TOP "))
                        sql = "SELECT TOP (" + top + ") " + sql.Substring(7);
                    else
                        sql = "SELECT TOP (" + top + ") " + sql.Substring(sql.IndexOf(")") + 1);
                    
                    sql = sql + (string.IsNullOrEmpty(filter) ? "" : " WHERE " + filter);
                    if (!string.IsNullOrEmpty(order)) sql += " ORDER BY " + order;
                }
            }
            else
            {
                sql = sql + (string.IsNullOrEmpty(filter) ? "" : " WHERE " + filter);
                if (!string.IsNullOrEmpty(order)) sql += " ORDER BY " + order;
            }

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sql, connection))
            {
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public bool Insert(Dictionary<string, object> data)
        {
            var provider = GetProvider();
            
            string insertString = "INSERT INTO " + m_table + " (";
            string valueString = "VALUES (";
            var parameters = new List<DbParameter>();

            foreach (var kvp in data)
            {
                if (kvp.Value != null)
                {
                    insertString += kvp.Key + ", ";
                    valueString += "@" + kvp.Key + ", ";
                    parameters.Add(provider.CreateParameter("@" + kvp.Key, kvp.Value));
                }
            }

            insertString = insertString.TrimEnd(',', ' ') + ")";
            valueString = valueString.TrimEnd(',', ' ') + ")";
            string sql = insertString + " " + valueString;

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sql, connection))
            {
                foreach (var param in parameters)
                    cmd.Parameters.Add(param);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public bool Update(int id, Dictionary<string, object> data)
        {
            var provider = GetProvider();
            
            string sql = "UPDATE " + m_table + " SET ";
            var parameters = new List<DbParameter>();

            foreach (var kvp in data)
            {
                if (kvp.Value != null)
                {
                    sql += kvp.Key + " = @" + kvp.Key + ", ";
                    parameters.Add(provider.CreateParameter("@" + kvp.Key, kvp.Value));
                }
            }

            sql = sql.TrimEnd(',', ' ') + " WHERE ID = @id";
            parameters.Add(provider.CreateParameter("@id", id));

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sql, connection))
            {
                foreach (var param in parameters)
                    cmd.Parameters.Add(param);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public void Delete(int id)
        {
            var provider = GetProvider();
            string sql = "DELETE FROM " + m_table + " WHERE ID = @id";

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sql, connection))
            {
                cmd.Parameters.Add(provider.CreateParameter("@id", id));
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute raw SQL and return scalar value
        /// </summary>
        public static object InvokeString(string sqlString)
        {
            var provider = GetProvider();

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sqlString, connection))
            {
                connection.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Execute raw SQL and return DataTable
        /// </summary>
        public static DataTable InvokeTString(string sqlString)
        {
            var provider = GetProvider();

            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sqlString, connection))
            {
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public DBObjectNew(string table, string selectString)
        {
            m_selectString = selectString;
            m_table = table;
        }
    }
}
