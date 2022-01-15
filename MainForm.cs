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

        private void íîâîåÏîäêëş÷åíèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewConnect nc = new NewConnect();
            nc.MdiParent = this;
            nc.Show();
        }

        private void âûõîäToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoginUsers_Click(object sender, EventArgs e)
        {
            LoginUsers w = new LoginUsers();
            w.MdiParent = this;
            w.Show();
        }

        private void ñäåëêèToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Orders w = new Orders();
            w.MdiParent = this;
            w.Show();
        }

        private void òîâàğûToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products w = new Products();
            w.MdiParent = this;
            w.Show();
        }

        private void ïîêóïàòåëèToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customers w = new Customers();
            w.MdiParent = this;
            w.Show();
        }

        private void êàñêàäîìToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void âåğòèêàëüíîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void ãîğèçîíòàëüíîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void îáíîâèòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataUpdate.updates();
        }

        private void çàêğûòüÂñåÎêíàToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllWindow.close();
        }

    }
}