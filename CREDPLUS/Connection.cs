using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace CREDPLUS
{
    class connection
    {
        MySqlConnection conn;
        string myConnectionString;
        static string host = "127.0.0.1";
        static string database = "crediplus";
        static string userDB = "";
        static string password = "";
        public static string strProvider = "server=" + host + ";Database=" + database + ";User ID=root;Password=;";

        public connection()
        {
            // Constructor, you can initialize default values here if needed
        }

        public MySqlConnection Open()
        {
            try
            {
                if (conn == null || conn.State == ConnectionState.Closed)
                {
                    myConnectionString = "server=" + host + ";Database=" + database + ";User ID=root;Password=;";
                    conn = new MySqlConnection(myConnectionString);
                    conn.Open();
                }

                return conn;
            }
            catch (Exception er)
            {
                MessageBox.Show("Connection Error! " + er.Message, "Information");
                return null;
            }
        }

        public void Close()
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public MySqlConnection GetConnection()
        {
            Open(); // Certifique-se de que a conexão esteja aberta antes de retornar
            return conn;
        }

        public DataSet ExecuteDataSet(string sql)
        {
            try
            {
                DataSet ds = new DataSet();

                if (conn != null)
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    da.Fill(ds, "result");
                    return ds;
                }
                else
                {
                    MessageBox.Show("The connection is null. Check the initialization of the connection.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            try
            {
                MySqlDataReader reader;

                if (conn != null)
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    reader = cmd.ExecuteReader();
                    return reader;
                }
                else
                {
                    MessageBox.Show("The connection is null. Check the initialization of the connection.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            try
            {
                int affected;

                if (conn != null)
                {
                    MySqlTransaction mytransaction = conn.BeginTransaction();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    affected = cmd.ExecuteNonQuery();
                    mytransaction.Commit();
                    return affected;
                }
                else
                {
                    MessageBox.Show("The connection is null. Check the initialization of the connection.");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
        }
    }
}
