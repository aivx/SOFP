using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Discounts : Form
    {
        public Discounts()
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
            new PackageClass() { Checked = true, name = "Идентификатор", ValueDisplayed = "[ID] AS [Идентификатор]"},
            new PackageClass() { Checked = true, name = "Скидка %", ValueDisplayed = "[Percentage] AS [Скидка %]"},
            new PackageClass() { Checked = true, name = "От", ValueDisplayed = "[From] AS [От]"},
            new PackageClass() { Checked = true, name = "До", ValueDisplayed = "[To] AS [До]"}
        };

        string select_str = $" [Discounts] ";
        string getDiscounts = "SELECT * FROM [Discounts] ";

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
            int id = 0;
            try { id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value); } catch { }
            DataTable dt = new DataTable();
            try
            {
                dt = StaticMethods.getTable(getDiscounts + $" WHERE ID={id}");
            }
            catch { }
            foreach (DataRow row in dt.Rows)
            {
                disc.Text = row[0].ToString();
                from.Text = row[1].ToString();
                to.Text = row[2].ToString();
            }
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
            if (disc.Text != "" && from.Text != "" && to.Text != "")
            {
                StaticMethods.NonQuery($"INSERT Discounts ([Percentage], [From], [To]) VALUES ({disc.Text}, {from.Text}, {to.Text})");
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (disc.Text != "" && from.Text != "" && to.Text != "")
            {
                int id = 0;
                try { id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value); } catch { }
                DataTable dt = new DataTable();
                try
                {
                    dt = StaticMethods.getTable(getDiscounts + $"WHERE A.ID={id}");
                }
                catch { }
                foreach (DataRow row in dt.Rows)
                {
                    if (!disc.Text.Equals(row[0].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{Connection.namedb}] SET [Percentage]='{disc.Text}' WHERE ID={id}");
                    if (!from.Text.Equals(row[1].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{Connection.namedb}] SET [From]='{from.Text}' WHERE ID={id}");
                    if (!to.Text.Equals(row[2].ToString()))
                        StaticMethods.NonQuery($"UPDATE [{Connection.namedb}] SET [To]='{to.Text}' WHERE ID={id}");
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }

        }
    }
}
