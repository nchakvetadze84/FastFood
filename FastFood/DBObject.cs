using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace FastFood
{
    public class DBObject
    {
        //private DataTable m_dataSource = new DataTable();
        private string m_table;
        private string m_selectString = String.Empty;
        private static string m_connectionString = LoadCSFromRegistry();

        //public DataTable DataSource
        //{
        //    get { return m_dataSource; }
        //    set { m_dataSource = value; }
        //}

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

        protected static string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            //set { m_connectionString = value; }
        }

        private static string LoadCSFromRegistry()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Connection");
            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey("Connection");
                rk.SetValue("ConnectionString", @"Data Source=localhost\SQLEXPRESS;Initial Catalog=FastFood;Integrated Security=True");
            }
            return (string)rk.GetValue("ConnectionString");
        }

        public DataTable Select(string filter, string order, int top)
        {
            if (top > 0)
            {
                if (!m_selectString.Contains(" TOP "))
                    m_selectString = "SELECT TOP (" + top.ToString() + ") " + m_selectString.Substring(7);
                else
                    m_selectString = "SELECT TOP (" + top.ToString() + ") " + m_selectString.Substring(m_selectString.IndexOf(")") + 1);
            }
            string sql = m_selectString + ((filter == "") ? "" : " where " + filter);
            if (order != String.Empty)
                sql += " ORDER BY " + order;
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);

            cmd.CommandType = CommandType.Text;

            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                DataTable returnTable = new DataTable();
                returnTable.Load(r);
                return returnTable;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool Insert(Dictionary<string, object> data)
        {
            string sql = String.Empty;
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            string insertString = "INSERT INTO " + m_table + " (";
            string valueString = "Values (";

            foreach (KeyValuePair<string, object> kvp in data)
            {
                if (kvp.Value != null)
                {
                    insertString += kvp.Key + " , ";
                    valueString += "@" + kvp.Key + ", ";

                    string param = "@" + kvp.Key;

                    SqlDbType sqlType = GetObjectSQLType(kvp.Value);
                    if (sqlType == SqlDbType.Udt)
                        return false;// new Exception("ვერ მოხერხერხდა სისტემური ტიპის გარდაქმნა მონაცემმთა ბაზის ტიპად");

                    cmd.Parameters.Add(param, sqlType);  //!!!
                    cmd.Parameters[param].Value = kvp.Value;
                }
            }

            insertString = insertString.Trim().TrimEnd(',') + ")";
            valueString = valueString.Trim().TrimEnd(',') + ")";

            sql = insertString + " " + valueString;

            try
            {
                connection.Open();
                cmd.CommandText = sql;
                cmd.ExecuteScalar();
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public bool Update(int id, Dictionary<string, object> data)
        {
            string sql = String.Empty;
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            string updatetString = "Update " + m_table + " ";
            string valueString = " SET ";

            foreach (KeyValuePair<string, object> kvp in data)
            {
                if (kvp.Value != null)
                {
                    valueString += kvp.Key + "= @" + kvp.Key + ", ";

                    string param = "@" + kvp.Key;

                    SqlDbType sqlType = GetObjectSQLType(kvp.Value);

                    cmd.Parameters.Add(param, sqlType);  //!!!
                    cmd.Parameters[param].Value = kvp.Value;
                }
            }

            string whereString = "WHERE ID = @id";

            cmd.Parameters.Add("@id", SqlDbType.Int);  //!!!
            cmd.Parameters["@id"].Value = id;


            valueString = valueString.Trim().TrimEnd(',');
            sql = updatetString + valueString + " " + whereString;

            try
            {
                connection.Open();
                cmd.CommandText = sql;
                cmd.ExecuteScalar();
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM " + Table + " WHERE (ID = @id)";
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add("@id", SqlDbType.Int);  //!!!
            cmd.Parameters["@id"].Value = id;

            try
            {
                connection.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public object InvokeProcedure(string procedureName, Dictionary<string, object> paramData)
        {
            string sql = procedureName;

            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlTransaction transaction = connection.BeginTransaction();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = transaction;

            foreach (KeyValuePair<string, object> kvp in paramData)
            {
                if (kvp.Value != null)
                {
                    SqlDbType sqlType = GetObjectSQLType(kvp.Value);
                    cmd.Parameters.Add(kvp.Key, sqlType);
                    cmd.Parameters[kvp.Key].Value = kvp.Value;
                }
            }
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                transaction.Commit();
                DataTable returnTable = new DataTable();
                returnTable.Load(r);
                return returnTable;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static object InvokeString(string sqlString)
        {
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            cmd.CommandType = CommandType.Text;

            try
            {
                connection.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static DataTable InvokeTString(string sqlString)
        {
            SqlConnection connection = new SqlConnection(m_connectionString);
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            cmd.CommandType = CommandType.Text;

            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                DataTable returnTable = new DataTable();
                returnTable.Load(r);
                return returnTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        private SqlDbType GetObjectSQLType(object o)
        {
            Type p = null;
            if (o != null)
                p = o.GetType();
            if (p == typeof(Char)) return SqlDbType.Bit;
            if (p == typeof(Byte[])) return SqlDbType.Binary;
            if (p == typeof(Decimal)) return SqlDbType.Decimal;
            if (p == typeof(UInt16) || p == typeof(UInt32) || p == typeof(Int16) || p == typeof(Int32)) return SqlDbType.Int;
            if (p == typeof(Int64) || p == typeof(UInt64)) return SqlDbType.BigInt;
            if (p == typeof(String) || p == typeof(Char)) return SqlDbType.NVarChar;
            if (p == typeof(Double) || p == typeof(float)) return SqlDbType.Float;
            if (p == typeof(DateTime)) return SqlDbType.DateTime;
            if (p == typeof(Boolean)) return SqlDbType.Bit;
            if (p == typeof(Guid)) return SqlDbType.UniqueIdentifier;
            if (p == typeof(Object)) return SqlDbType.Variant;

            return SqlDbType.Udt;
        }

        public DBObject(string table, string selectString)
        {
            m_selectString = selectString;
            m_table = table;
        }
    }    
}
