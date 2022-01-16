using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Linq.Enumerable;

namespace SOFP
{
    public partial class Products : Form
    {
        public Products()
        {
            InitializeComponent();
            CloseAllWindow.GetClose += CloseAllWindow_GetClose;
        }

        private void CloseAllWindow_GetClose()
        {
            this.Close();
        }
        List<PackageClass> list;
        void getData()
        {
            dataGridView1.DataSource = StaticMethods.getTable($"SELECT {StaticMethods.setTables(list)} FROM [Products]");
        }
        private void Products_Load(object sender, EventArgs e)
        {
            list = new List<PackageClass>()
            {
                new PackageClass() {Checked = true, ValueDisplayed = "ID"},
                new PackageClass() {Checked = true, ValueDisplayed = "Name"},
                new PackageClass() {Checked = true, ValueDisplayed = "Description"}
            };
            checkedListBox1.DataSource = list;
            checkedListBox1.DisplayMember = "ValueDisplayed";
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
            getData();
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            getData();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
