using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOFP
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            NewConnect nc = new NewConnect();
            nc.MdiParent = this;
            nc.Show();
        }

        private void ����������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewConnect nc = new NewConnect();
            nc.MdiParent = this;
            nc.Show();
        }

        private void �����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoginUsers_Click(object sender, EventArgs e)
        {
            LoginUsers w = new LoginUsers();
            w.MdiParent = this;
            w.Show();
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Orders w = new Orders();
            w.MdiParent = this;
            w.Show();
        }

        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products w = new Products();
            w.MdiParent = this;
            w.Show();
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customers w = new Customers();
            w.MdiParent = this;
            w.Show();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void �����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void �������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataUpdate.updates();
        }

        private void ��������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllWindow.close();
        }

    }
}