using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Customers : Form
    {
        public Customers()
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
            new PackageClass() { Checked = false, name = "Идентификатор", ValueDisplayed = "A.[ID] AS [Идентификатор]"},
            new PackageClass() { Checked = true, name = "Фамилия", ValueDisplayed = "[FName] AS [Фамилия]"},
            new PackageClass() { Checked = true, name = "Имя", ValueDisplayed = "A.[MName] AS [Имя]"},
            new PackageClass() { Checked = true, name = "Отчество", ValueDisplayed = "A.[LName] AS [Отчество]"},
            new PackageClass() { Checked = true, name = "Псевдоним", ValueDisplayed = "A.[Nname] AS [Псевдоним]"},
            new PackageClass() { Checked = true, name = "Адрес", ValueDisplayed = "A.[Address] AS [Адрес]"},
            new PackageClass() { Checked = true, name = "Телефон", ValueDisplayed = "A.[Phone] AS [Телефон]"}
        };
        string namedb = "Customers";
        string select_str = " Customers A";
        string getCustomers = "SELECT [FName],A.[MName],A.[LName],A.[Nname],A.[Address],A.[Phone] FROM Customers A ";

        DataTable table = new DataTable();
        int cursor = 0;

        void getData()
        {
            try
            {
                cursor = dataGridView1.CurrentCell.RowIndex;
            }
            catch { }
            table = StaticMethods.getTable($"SELECT {StaticMethods.setTables(listTables)} FROM {select_str}");
            BindingSource bs = new BindingSource();
            bs.DataSource = table;
            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
            dataGridView1.CurrentCell = dataGridView1[0, cursor];
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
            List<string> idList = StaticMethods.getIDList($"SELECT [ID] FROM [{namedb}]");
           // int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            StaticMethods.NonQuery($"DELETE FROM [{namedb}] WHERE ID={idList[dataGridView1.CurrentCell.RowIndex]}");
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                return;
            }
            if (cursor != dataGridView1.CurrentCell.RowIndex)
            {
                table.DefaultView.Sort = string.Empty;
            }
            List<string> idList = StaticMethods.getIDList($"SELECT ID FROM [{namedb}]");
            DataTable dt = new DataTable();
            try
            {
                dt = StaticMethods.getTable(getCustomers + $"WHERE A.ID={idList[dataGridView1.CurrentCell.RowIndex]}");
            }
            catch { }
            foreach (DataRow row in dt.Rows)
            {
                FName.Text = row[0].ToString();
                MName.Text = row[1].ToString();
                LName.Text = row[2].ToString();
                Nname.Text = row[3].ToString();
                address.Text = row[4].ToString();
                phone.Text = row[5].ToString();
            }
        }

        private void prod_SelectedIndexChanged(object sender, EventArgs e)
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
            if (FName.Text != "" && MName.Text != "" && LName.Text != "" && Nname.Text != "" && address.Text != "" && phone.Text != "")
            {
                string strcmd = 
                    $"INSERT {namedb} " +
                    $"([FName],[MName],[LName],[Nname],[Address],[Phone])" +
                    $"VALUES ('{FName.Text}','{MName.Text}','{LName.Text}','{Nname.Text}','{address.Text}','{phone.Text}')";
                StaticMethods.NonQuery(strcmd);
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (FName.Text != "" && MName.Text != "" && LName.Text != "" && Nname.Text != "" && address.Text != "" && phone.Text != "")
            {
                List<string> idList = StaticMethods.getIDList($"SELECT ID FROM [{namedb}]");
                string id = idList[dataGridView1.CurrentCell.RowIndex];
                if (dataGridView1.CurrentRow == null)
                {
                    return;
                }
                DataTable dt = new DataTable();
                try
                {
                    dt = StaticMethods.getTable(getCustomers + $"WHERE A.ID={idList[dataGridView1.CurrentCell.RowIndex]}");
                }
                catch { }
                foreach (DataRow row in dt.Rows)
                {
                    if (!FName.Text.Equals(row[0].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [FName]='{FName.Text}' WHERE ID={id}");
                    if (!MName.Text.Equals(row[1].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [MName]='{MName.Text}' WHERE ID={id}");
                    if (!LName.Text.Equals(row[2].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [LName]='{LName.Text}' WHERE ID={id}");
                    if (!Nname.Text.Equals(row[3].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [Nname]='{Nname.Text}' WHERE ID={id}");
                    if (!address.Text.Equals(row[4].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [Address]='{address.Text}' WHERE ID={id}");
                    if (!phone.Text.Equals(row[5].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{namedb}] SET [Phone]='{phone.Text}' WHERE ID={id}");
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }
    }
}
