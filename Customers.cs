using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFP
{
    public partial class Customers : Form
    {
        public Customers()
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
            dataGridView1.DataSource = StaticMethods.getData("Customers", list);
        }

        private void DataUpdate_GetUpdate()
        {
            getData();
        }
        private void CustomersForm_Load(object sender, EventArgs e)
        {
            list = new List<PackageClass>()
            {
                new PackageClass() {Checked = true, ValueDisplayed = "ID"},
                new PackageClass() {Checked = true, ValueDisplayed = "FName"},
                new PackageClass() {Checked = true, ValueDisplayed = "MName"},
                new PackageClass() {Checked = true, ValueDisplayed = "LName"},
                new PackageClass() {Checked = true, ValueDisplayed = "Nname"},
                new PackageClass() {Checked = true, ValueDisplayed = "Address"},
                new PackageClass() {Checked = true, ValueDisplayed = "Phone"}
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
    }
}
