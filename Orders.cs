using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            DataUpdate.GetUpdate += DataUpdate_GetUpdate;
            dataGridView1.DataError += DataGridView1_DataError;
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        string nameTable = "Orders";
        List<PackageClass> list = new List<PackageClass>()
        {
            new PackageClass() { Checked = true, name = "ID сделки", ValueDisplayed = "F.ID AS [ID сделки]"},
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

        void getData()
        {
            
            BindingSource bs = new BindingSource();
            bs.DataSource = StaticMethods.getData(select_str, list);
            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
            cust.Items.Clear();
            prod.Items.Clear();
            DataTable dt = StaticMethods.getReq($"SELECT [Nname] FROM [Customers]");
            foreach (DataRow row in dt.Rows)
            {
                cust.Items.Add(row[0].ToString());
            }
            dt = StaticMethods.getReq($"SELECT [Name] FROM [Products]");
            foreach (DataRow row in dt.Rows)
            {
                prod.Items.Add(row[0].ToString());
            }
        }
        private void Drivers_Load(object sender, EventArgs e)
        {
            checkedListBox1.DataSource = list;
            checkedListBox1.DisplayMember = "name";
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                checkedListBox1.SetItemChecked(i, ((PackageClass)checkedListBox1.Items[i]).Checked);
            }
            checkedListBox1.ItemCheck += (sender, e) => {
                list[e.Index].Checked = (e.NewValue != CheckState.Unchecked);
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
            StaticMethods.NonQuery($"DELETE FROM {nameTable} WHERE ID={id.ToString()}");

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                return;
            }
            DataTable dt = StaticMethods.getReq($"SELECT ID FROM [Customers] WHERE [Nname] LIKE '{cust.SelectedItem.ToString()}' ");
            string c="",p = "";
            foreach (DataRow row in dt.Rows)
            {
                c = row[0].ToString();
            }
            dt = StaticMethods.getReq($"SELECT ID FROM [Products] WHERE [Name] LIKE '{prod.SelectedItem.ToString()}' ");
            foreach (DataRow row in dt.Rows)
            {
                p = row[0].ToString();
            }
            string ss = $"EXEC [add_Order] {c},{p},{count.Text} ";
            StaticMethods.NonQuery(ss); 
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            List<string> lst = new List<string>();
            DataTable dt = StaticMethods.getReq("SELECT ID FROM[Orders]");
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(row[0].ToString());
            }
            
            dt = StaticMethods.getReq(
                $"SELECT B.[FName],B.[MName],B.[LName],B.[Nname],B.[Address],B.[Phone]" +
                $"FROM [Orders] A INNER JOIN [Customers] B ON A.[CustomerID] = B.[ID]" +
                $"WHERE A.ID={lst[dataGridView1.CurrentCell.RowIndex]}");
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
            DataTable dt = StaticMethods.getReq($"SELECT A.[Count] FROM [Stocks] A INNER JOIN [Products] B ON A.[ProductID] = B.[ID] WHERE B.Name LIKE '{prod.SelectedItem.ToString()}'");
            foreach (DataRow row in dt.Rows)
            {
                countStocks.Text = row[0].ToString();
            }
        }
    }
}
