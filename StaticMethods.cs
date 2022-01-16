
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Linq.Enumerable;

namespace SOFP
{
    public static class StaticMethods
    {
        public static DataTable getTable(string str)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(str, Connection.sqlConnection);
                DataSet dataset = new DataSet();
                dataAdapter.Fill(dataset);
                dt = dataset.Tables[0];
            }
            catch (SqlException odbcEx)
            {
                MessageBox.Show(odbcEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }


        public static string setTables(List<PackageClass> vs)
        {
            string rows = "";
            bool check = false;
            foreach (var (value, i) in vs.Select((value, i) => (value, i)))
            {
                if (value.Checked)
                {
                    rows += value.ValueDisplayed;
                    rows += ", ";
                    check = true;
                }
            }
            if (check)
                rows = rows.Remove((rows.Length - 2), 2);
            else
                rows = "*";

            return rows;
        }


        public static void NonQuery(string str)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand(str, Connection.sqlConnection);
                command.ExecuteNonQuery();
                DataUpdate.updates();
                StatusUpdate.setStatus("Операция выполнена успешно");
            }
            catch (SqlException odbcEx)
            {
                StatusUpdate.setStatus("Последняя операция выполнена с ошибкой");
                string mess;
                if (odbcEx.Number == 547)
                    mess = "Сделка не состоялась! \nКоличества товара не хватает на складе";
                else
                    mess = odbcEx.Message;
                MessageBox.Show(mess);
            }
        }
        public static List<string> getIDList(string str)
        {
            List<string> idList = new List<string>();
            DataTable dt = getTable(str);
            foreach (DataRow row in dt.Rows)
            {
                idList.Add(row[0].ToString());
            }
            return idList;
        }

    }
}
