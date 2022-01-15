using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SOFP
{
    public partial class LoginUsers : Form
    {
        public LoginUsers()
        {
            InitializeComponent();
            dataGridView1.DataError += DataGridView1_DataError;
            CloseAllWindow.GetClose += CloseAllWindow_GetClose;
        }

        private void CloseAllWindow_GetClose()
        {
            this.Close();
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void LoginUsers_Load(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from SOFP4.sys.sql_logins", Connection.sqlConnection);
            DataSet dataset = new DataSet();
            dataAdapter.Fill(dataset);
            dataGridView1.DataSource = dataset.Tables[0];
        }
    }
}
