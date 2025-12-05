using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;

namespace FastFood
{
    class User
    {
        public static int CheckPass(string user, string password) //returns UserID
        {
            int id = Convert.ToInt32(DBObject.InvokeString("SELECT ID FROM dbo.Users WHERE USER_NAME = '" + user + "' AND USER_PASSWORD = '"+password+"'"));
            
            //cmd.Parameters.Add("@LoginName", SqlDbType.VarChar, 20);
            //cmd.Parameters["@LoginName"].Value = user;
            //cmd.Parameters.Add("@LoginPass", SqlDbType.Char, 16);
            //cmd.Parameters["@LoginPass"].Value = password;

            try
            {
                //connection.Open();
                return Convert.ToInt32(id);
            }
            finally
            {
                //connection.Close();
            }
        }

        public static Byte[] EncriptPass(string pass)
        {
            Byte[] hashedDataBytes;
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            hashedDataBytes = md5Hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(pass));
            return hashedDataBytes;
        }

        public static string GetString(byte[] charArray)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charArray.Length; i++)
            {
                sb.Append(charArray[i].ToString("X2"));
            }

           
            return sb.ToString();

        }
    }
}
