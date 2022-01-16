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
        public static SqlConnection sqlConnection = new SqlConnection();
        public static bool newConnect(string source, string login, string password)
        {

            string ConnectionString = $"Persist Security Info=False;User ID={login};Password={password};Server={source}";
            if (login == "")
            {
                ConnectionString = $"Data Source={source}; Integrated Security=True";
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
