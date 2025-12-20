using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Data.Common;

namespace FastFood
{
    /// <summary>
    /// User authentication class - UPDATED with security fixes
    /// 
    /// Changes:
    /// 1. Fixed SQL Injection vulnerability - now uses parameterized queries
    /// 2. MD5 is deprecated - BCrypt recommended for production
    /// 3. Uses new database provider abstraction
    /// </summary>
    class UserSecure
    {
        /// <summary>
        /// Check user password and return UserID
        /// FIXED: Uses parameterized query to prevent SQL Injection
        /// </summary>
        public static int CheckPass(string user, string password)
        {
            // SECURE: Parameterized query prevents SQL injection
            string sql = "SELECT ID FROM dbo.Users WHERE USER_NAME = @user AND USER_PASSWORD = @password";
            
            var provider = GetProvider();
            
            using (var connection = provider.CreateConnection())
            using (var cmd = provider.CreateCommand(sql, connection))
            {
                cmd.Parameters.Add(provider.CreateParameter("@user", user));
                cmd.Parameters.Add(provider.CreateParameter("@password", password));

                connection.Open();
                var result = cmd.ExecuteScalar();
                
                if (result == null || result == DBNull.Value)
                    return -1;
                    
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Hash password using MD5 (legacy - keep for backward compatibility)
        /// WARNING: MD5 is cryptographically broken. Use BCrypt for new implementations.
        /// </summary>
        [Obsolete("MD5 is deprecated. Use HashPasswordSecure with BCrypt for new implementations.")]
        public static byte[] EncriptPass(string pass)
        {
            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                return md5Hasher.ComputeHash(Encoding.ASCII.GetBytes(pass));
            }
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        public static string GetString(byte[] charArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in charArray)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get database provider (simplified for demo)
        /// In production, use dependency injection
        /// </summary>
        private static IDatabaseProvider GetProvider()
        {
            // This would typically come from configuration or DI
            // For demo purposes, using SQL Server
            return new SqlServerProvider 
            { 
                ConnectionString = LoadConnectionString() 
            };
        }

        private static string LoadConnectionString()
        {
            var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Connection");
            if (rk == null)
            {
                rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Connection");
                rk.SetValue("ConnectionString", 
                    @"Data Source=localhost\SQLEXPRESS;Initial Catalog=FastFood;Integrated Security=True");
            }
            return (string)rk.GetValue("ConnectionString");
        }

        #region BCrypt Implementation (Recommended for Production)
        
        /*
         * To use BCrypt, install BCrypt.Net-Next NuGet package:
         * Install-Package BCrypt.Net-Next
         * 
         * Example usage:
         * 
         * // Hash password
         * string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
         * 
         * // Verify password
         * bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
         */

        /// <summary>
        /// Secure password hashing using BCrypt (placeholder)
        /// Uncomment and use BCrypt.Net-Next package in production
        /// </summary>
        public static string HashPasswordSecure(string password)
        {
            // TODO: Install BCrypt.Net-Next and uncomment:
            // return BCrypt.Net.BCrypt.HashPassword(password);
            
            // Temporary: Use SHA256 (better than MD5, but BCrypt is recommended)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return GetString(hashedBytes);
            }
        }

        /// <summary>
        /// Verify password against stored hash (placeholder for BCrypt)
        /// </summary>
        public static bool VerifyPasswordSecure(string password, string storedHash)
        {
            // TODO: With BCrypt:
            // return BCrypt.Net.BCrypt.Verify(password, storedHash);
            
            // Temporary: SHA256 comparison
            return HashPasswordSecure(password) == storedHash;
        }

        #endregion
    }
}
