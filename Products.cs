using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Products : Form
    {
        public Products()
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
            new PackageClass() { Checked = true, name = "Идентификатор", ValueDisplayed = "A.[ID] AS [Идентификатор]"},
            new PackageClass() { Checked = true, name = "Название", ValueDisplayed = "A.[Name] AS [Название]"},
            new PackageClass() { Checked = true, name = "Описание", ValueDisplayed = "A.[Description] AS [Описание]"},
            new PackageClass() { Checked = true, name = "Оптовая цена", ValueDisplayed = "B.[WPrice] AS [Оптовая цена]"},
            new PackageClass() { Checked = true, name = "Розничная цена", ValueDisplayed = "B.[RPrice] AS [Розничная цена]"},
            new PackageClass() { Checked = true, name = "Скидка %", ValueDisplayed = "C.[Percentage] AS [Скидка %]"},
            new PackageClass() { Checked = true, name = "От", ValueDisplayed = "C.[From] AS [От]"},
            new PackageClass() { Checked = true, name = "До", ValueDisplayed = "C.[To] AS [До]"},
            new PackageClass() { Checked = true, name = "Количество", ValueDisplayed = "D.[Count] AS [Количество]"}
        };
        string select_str =
            "  [Products] A " +
            "INNER JOIN [ProductPrice] B ON A.[ID] = B.[ProductID] " +
            "INNER JOIN [Discounts] C ON B.[DiscountID] = C.[ID] " +
            "INNER JOIN [Stocks] D ON A.[ID] = D.[ProductID] ";

        DataTable table = new DataTable();

        void getData()
        {
            table = StaticMethods.getTable($"SELECT {StaticMethods.setTables(listTables)} FROM {select_str}");
            BindingSource bs = new BindingSource();
            bs.DataSource = table;
            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
            discountsID.Items.Clear();
            DataTable dt = StaticMethods.getTable($"SELECT [Percentage],[From],[TO] FROM [Discounts]");
            foreach (DataRow row in dt.Rows)
            {
                discountsID.Items.Add($"{row[0].ToString()}% От {row[1].ToString()} До {row[2].ToString()}");
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

        private void DataUpdate_GetUpdate()
        {
            if (!this.IsDisposed)
            {
                getData();
            }
        }

        private void removeItem(object sender, EventArgs e)
        {
            int id = 0;
            try { id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value); } catch { }
            // int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            StaticMethods.NonQuery($"DELETE FROM [Products] WHERE ID={id}");
        }

        string getProduct =
                $"SELECT A.[Name], A.[Description], B.[WPrice], B.[RPrice], C.[Percentage], C.[From], C.[To], D.[Count]" +
                $"FROM [Products] A " +
                $"INNER JOIN [ProductPrice] B ON A.[ID] = B.[ProductID]" +
                $"INNER JOIN [Discounts] C ON B.[DiscountID] = C.[ID]" +
                $"INNER JOIN [Stocks] D ON A.[ID] = D.[ProductID]";


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int id = 0;
            try { id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value); } catch { }
            DataTable dt = new DataTable();
            try
            {
                dt = StaticMethods.getTable(getProduct + $" WHERE A.ID={id}");
            }
            catch { }
            foreach (DataRow row in dt.Rows)
            {
                nameProd.Text = row[0].ToString();
                discriptProd.Text = row[1].ToString();
                wPriceProd.Text = row[2].ToString();
                rPriceProd.Text = row[3].ToString();
                discountsID.Text = $"{row[4].ToString()}% От {row[5].ToString()} До {row[6].ToString()}";
                countProd.Text = row[7].ToString();
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            List<string> idList = StaticMethods.getIDList("SELECT ID FROM [Discounts]");
            if (nameProd.Text != "" && discriptProd.Text != "" && rPriceProd.Text != "" && wPriceProd.Text != "" && countProd.Text != "")
            {
                try
                {
                    StaticMethods.NonQuery($"EXEC [add_Product] '{nameProd.Text}','{discriptProd.Text}',{wPriceProd.Text.Replace(',', '.')}," +
                    $"{rPriceProd.Text.Replace(',', '.')}, {idList[discountsID.SelectedIndex]},{countProd.Text}");
                }
                catch(Exception ex) { MessageBox.Show(ex.Message); }    
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (nameProd.Text != "" && discriptProd.Text != "" && rPriceProd.Text != "" && wPriceProd.Text != "" &&
                discountsID.SelectedItem.ToString() != "" && countProd.Text != "") 
            {
                int id = 0;
                try { id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value); } catch { }
                List<string> idList = StaticMethods.getIDList("SELECT ID FROM [Discounts]");
                DataTable dt = new DataTable();
                try
                {
                    dt = StaticMethods.getTable(getProduct + $" WHERE A.ID={id}");
                }
                catch { }
                foreach (DataRow row in dt.Rows)
                {
                    if (!nameProd.Text.Equals(row[0].ToString()))
                        StaticMethods.NonQuery($"UPDATE [Products] SET [Name]='{nameProd.Text}' WHERE ID={id}");
                    if (!discriptProd.Text.Equals(row[1].ToString()))
                        StaticMethods.NonQuery($"UPDATE [Products] SET [Description]='{discriptProd.Text}' WHERE ID={id}");
                    if (!wPriceProd.Text.Equals(row[2].ToString()))
                        StaticMethods.NonQuery($"UPDATE [ProductPrice] SET [WPrice]='{wPriceProd.Text.Replace(',', '.')}' WHERE ProductID={id}");
                    if (!rPriceProd.Text.Equals(row[3].ToString()))
                        StaticMethods.NonQuery($"UPDATE [ProductPrice] SET [RPrice]='{rPriceProd.Text.Replace(',', '.')}' WHERE ProductID={id}");
                    if (!idList[discountsID.SelectedIndex].Equals(row[4].ToString()))
                        StaticMethods.NonQuery($"UPDATE [ProductPrice] SET [DiscountID]='{idList[discountsID.SelectedIndex]}' WHERE ProductID={id}");
                    if (!countProd.Text.Equals(row[7].ToString()))
                        StaticMethods.NonQuery($"UPDATE [Stocks] SET [Count]='{countProd.Text}' WHERE ProductID={id}");
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }
    }
}
