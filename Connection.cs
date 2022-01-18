using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFP
{
    static internal class Connection
    {
        public static string source { get; private set; }
        public static string namedb { get; private set; }
        public static string login { get; private set; }
        public static string password { get; private set; }
        public static SqlConnection sqlConnection = new SqlConnection();

        public static void newConnect(string s, string db, string l, string p)
        {
            source = s;
            namedb = db;
            login = l;
            password = p;
        }

        public static bool openConnections()
        {
            string ConnectionString = $"Persist Security Info=False;User ID={login};Password={password};Initial Catalog={namedb};Server={source}";
            if (login == "")
            {
                ConnectionString = $"Initial Catalog={namedb};Data Source={source}; Integrated Security=True";
            }
            sqlConnection = new SqlConnection(ConnectionString);
            try
            {
                sqlConnection.Open();
                StatusUpdate.setStatus("Подключено");
            }
            catch
            {
                sqlConnection.Close();
                StatusUpdate.setStatus("Не удалось подключиться к серверу");
                MessageBox.Show("Ошибка подключения!");
                return false;
            }
            return true;
        }
        public static void closeConnections()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    sqlConnection.Close();
                    StatusUpdate.setStatus("Соединение успешно закрыто");
                }
                catch (SqlException odbcEx)
                {
                    StatusUpdate.setStatus("Последняя операция выполнена с ошибкой");
                }
            }
        }
    }
}
