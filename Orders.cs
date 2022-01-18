using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
            DataUpdate.GetUpdate += DataUpdate_GetUpdate;
            dataGridView1.DataError += DataGridView1_DataError;
        }
        private void DataUpdate_GetUpdate()
        {
            if (!this.IsDisposed)
            {
                getData();
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        List<PackageClass> listTables = new List<PackageClass>()
        {
            new PackageClass() { Checked = false, name = "ID сделки", ValueDisplayed = "F.ID AS [ID сделки]"},
            new PackageClass() { Checked = true, name = "Псевдоним покупателя", ValueDisplayed = "G.[LName] AS [Псевдоним покупателя]"},
            new PackageClass() { Checked = true, name = "Наименование товара", ValueDisplayed = "A.[Name] AS [Наименование товара]"},
            new PackageClass() { Checked = true, name = "Описание товара", ValueDisplayed = "A.[Description] AS [Описание товара]"},
            new PackageClass() { Checked = true, name = "Количество куплено", ValueDisplayed = "E.[Count] AS [Количество куплено]"},
            new PackageClass() { Checked = true, name = "Количество на складе", ValueDisplayed = "B.[Count] AS [Количество на складе]"},
            new PackageClass() { Checked = true, name = "Cкидка", ValueDisplayed = "[Percentage] AS [Cкидка]"},
            new PackageClass() { Checked = true, name = "От", ValueDisplayed = "C.[From] AS [От]"},
            new PackageClass() { Checked = true, name = "До", ValueDisplayed = "C.[To] AS [До]"},
            new PackageClass() { Checked = true, name = "Розничная цена", ValueDisplayed = "D.[RPrice] AS [Розничная цена]"},
            new PackageClass() { Checked = true, name = "Оптовая цена", ValueDisplayed = "D.[WPrice] AS [Оптовая цена]"},
            new PackageClass() { Checked = true, name = "Цена с учетом скидки", ValueDisplayed = "E.[Price] AS [Цена с учетом скидки]"},
            new PackageClass() { Checked = true, name = "Дата сделки", ValueDisplayed = "F.[OrderDate] AS [Дата сделки]"}
        };
        string select_str =
            " Products A " +
            "INNER JOIN Stocks B ON A.ID = B.ProductID " +
            "INNER JOIN ProductDiscount C ON A.ID = C.ProductID " +
            "INNER JOIN ProductPrice D ON A.ID = D.ProductID " +
            "INNER JOIN OrderDetails E ON A.ID = E.ProductID " +
            "INNER JOIN Orders F ON F.ID = E.OrderID " +
            "INNER JOIN Customers G ON F.[CustomerID] = G.[ID]";

        DataTable table = new DataTable();

        int cursor = 0;

        void getData()
        {
            try { cursor = dataGridView1.CurrentCell.RowIndex; } catch { }
            table = StaticMethods.getTable($"SELECT {StaticMethods.setTables(listTables)} FROM {select_str}");
            BindingSource bs = new BindingSource();
            bs.DataSource = table;
            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
            try { dataGridView1.CurrentCell = dataGridView1[0, cursor]; } catch { }
            cust.Items.Clear();
            prod.Items.Clear();
            DataTable dt = StaticMethods.getTable($"SELECT [Nname] FROM [Customers]");
            foreach (DataRow row in dt.Rows)
            {
                cust.Items.Add(row[0].ToString());
            }
            dt = StaticMethods.getTable($"SELECT [Name] FROM [Products]");
            foreach (DataRow row in dt.Rows)
            {
                prod.Items.Add(row[0].ToString());
            }
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

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            List<string> idList = StaticMethods.getIDList("SELECT ID FROM [Orders]");
           // int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            StaticMethods.NonQuery($"DELETE FROM [Orders] WHERE ID={idList[dataGridView1.CurrentCell.RowIndex]}");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                return;
            }
            if (cursor != dataGridView1.CurrentCell.RowIndex)
            {
                table.DefaultView.Sort = string.Empty;
            }
            DataTable dt = new DataTable();
            try
            {
                dt = StaticMethods.getTable($"SELECT ID FROM [Customers] WHERE [Nname] LIKE '{cust.SelectedItem.ToString()}' ");
                string c = "", p = "";
                foreach (DataRow row in dt.Rows)
                {
                    c = row[0].ToString();
                }
                dt = StaticMethods.getTable($"SELECT ID FROM [Products] WHERE [Name] LIKE '{prod.SelectedItem.ToString()}' ");
                foreach (DataRow row in dt.Rows)
                {
                    p = row[0].ToString();
                }
                string ss = $"EXEC [add_Order] {c},{p},{count.Text} ";
                StaticMethods.NonQuery(ss);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            List<string> idList = StaticMethods.getIDList("SELECT ID FROM [Orders]");
            DataTable dt = new DataTable();
            try
            {
                dt = StaticMethods.getTable(
                    $"SELECT B.[FName],B.[MName],B.[LName],B.[Nname],B.[Address],B.[Phone]" +
                    $"FROM [Orders] A INNER JOIN [Customers] B ON A.[CustomerID] = B.[ID]" +
                    $"WHERE A.ID={idList[dataGridView1.CurrentCell.RowIndex]}");
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
            DataTable dt = StaticMethods.getTable($"SELECT A.[Count] FROM [Stocks] A INNER JOIN [Products] B ON A.[ProductID] = B.[ID] WHERE B.Name LIKE '{prod.SelectedItem.ToString()}'");
            foreach (DataRow row in dt.Rows)
            {
                countStocks.Text = row[0].ToString();
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
    }
}
