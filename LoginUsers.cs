using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SOFP
{
    public partial class LoginUsers : Form
    {
        public LoginUsers()
        {
            InitializeComponent();
            DataUpdate.GetUpdate += DataUpdate_GetUpdate;
            dataGridView1.DataError += DataGridView1_DataError;
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
        List<PackageClass> listTables = new List<PackageClass>()
        {
            new PackageClass() { Checked = true, name = "Идентификатор", ValueDisplayed = "[principal_id] AS [Идентификатор]"},
            new PackageClass() { Checked = true, name = "Имя", ValueDisplayed = "[name] AS [Имя]"},
            new PackageClass() { Checked = true, name = "Тип", ValueDisplayed = "[type_desc]AS [Тип]"},
            new PackageClass() { Checked = true, name = "База данных по умолчанию", ValueDisplayed = "[default_database_name] AS [База данных по умолчанию]"},
            new PackageClass() { Checked = true, name = "Язык по умолчанию", ValueDisplayed = "[default_language_name]AS [Язык по умолчанию]"},
            new PackageClass() { Checked = true, name = "Дата создания", ValueDisplayed = "[create_date] AS [Дата создания]"},
            new PackageClass() { Checked = true, name = "Дата изменения", ValueDisplayed = "[modify_date] AS [Дата изменения]"},
            new PackageClass() { Checked = true, name = "Отключен", ValueDisplayed = "[is_disabled]AS [Отключен]"}
        };

        string select_str = $" {Connection.namedb}.sys.sql_logins ";

        DataTable table = new DataTable();

        void getData()
        {
            table = StaticMethods.getTable($"SELECT {StaticMethods.setTables(listTables)} FROM {select_str}");
            BindingSource bs = new BindingSource();
            bs.DataSource = table;
            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
        }
        private void Drivers_Load(object sender, EventArgs e)
        {
            checkedListBox1.DataSource = listTables;
            checkedListBox1.DisplayMember = "name";
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                checkedListBox1.SetItemChecked(i, ((PackageClass)checkedListBox1.Items[i]).Checked);
            }
            checkedListBox1.ItemCheck += (sender, e) => {
                listTables[e.Index].Checked = (e.NewValue != CheckState.Unchecked);
            };
            DataUpdate.GetUpdate += DataUpdate_GetUpdate;
            getData();
        }

        private void DataUpdate_GetUpdate()
        {
            if (!this.IsDisposed)
            {
                getData();
            }
        }

        private void removeItem(object sender, EventArgs e)
        {
            string name = "";
            try { name = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString(); } catch { }
            // int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            StaticMethods.NonQuery(
                    $"USE [{Connection.namedb}]; DROP LOGIN [{name}]; " +
                    $"USE [{Connection.namedb}]; DROP USER [{name}]");
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            StaticMethods.setTables(listTables);
            getData();
            button1_Click(sender, e);
            StaticMethods.setTables(listTables);
        }


        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (login.Text != "" && password.Text != "" && password2.Text != "")
            {
                if (password.Text != password2.Text)
                {
                    MessageBox.Show("пароли не совпадают");
                }
                else
                {
                    string strcmd = $"USE [{Connection.namedb}]; CREATE LOGIN [{login.Text}] WITH PASSWORD=N'{password.Text}', DEFAULT_DATABASE=[{Connection.namedb}], DEFAULT_LANGUAGE=[русский], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF; "+
                    $"USE [{Connection.namedb}]; CREATE USER [{login.Text}] FOR LOGIN [{login.Text}] WITH DEFAULT_SCHEMA=[dbo]; ";
                    if (getGrant.SelectedIndex == 0)
                    {
                        strcmd += $"USE [{Connection.namedb}]; ALTER ROLE [db_datareader] ADD MEMBER [{login.Text}]; ";
                    }
                    else if (getGrant.SelectedIndex == 1)
                    {
                        strcmd += $"USE [{Connection.namedb}]; ALTER ROLE [db_datareader] ADD MEMBER [{login.Text}]; ";
                        strcmd += $"USE [{Connection.namedb}]; ALTER ROLE [db_datawriter] ADD MEMBER [{login.Text}]; ";
                    }
                    else if (getGrant.SelectedIndex == 2)
                    {
                        strcmd += $"USE [{Connection.namedb}]; ALTER ROLE [db_accessadmin] ADD MEMBER [{login.Text}]; ";
                    }
                    StaticMethods.NonQuery(strcmd);
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }
    }
}
