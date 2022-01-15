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
        public static bool newconnect(string source, string login, string password)
        {
            string ConnectionString = String.Format(
                "Data Source={0}; User ID={1}; Password={2};",
                source, login, password);
            if(login == "")
            {
                ConnectionString = String.Format(
                "Data Source={0}; Integrated Security=True",
                source);
            }
            sqlConnection = new SqlConnection(ConnectionString);
            try { 
                sqlConnection.Open();
            } catch { 
                sqlConnection.Close();
                MessageBox.Show("Ошибка подключения!");
                return false;
            }
            return true;
        }
    }
}
