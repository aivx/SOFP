using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void ShowNewForm(object sender, EventArgs e)
        {
            NewConnect nc = new NewConnect();
            nc.MdiParent = this;
            nc.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }


        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{x}");
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{v}");
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^{c}");
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void MDIParent1_Load(object sender, EventArgs e)
        {
            ShowNewForm(sender, e);
        }

        private void printPreviewToolStripButton_Click(object sender, EventArgs e)
        {
            DataUpdate.updates();
        }

        private void покупателиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customers nc = new Customers();
            nc.MdiParent = this;
            nc.Show();
        }

        private void сделкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Orders nc = new Orders();
            nc.MdiParent = this;
            nc.WindowState = FormWindowState.Maximized;
            nc.Show();
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Products nc = new Products();
            nc.MdiParent = this;
            nc.Show();
        }

        private void системаСкидокToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void открытьВсеОкнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            покупателиToolStripMenuItem_Click(sender, e);
            Orders nc = new Orders();
            nc.MdiParent = this;
            nc.Show();
            товарыToolStripMenuItem_Click(sender, e);
            системаСкидокToolStripMenuItem_Click(sender, e);
            LayoutMdi(MdiLayout.Cascade);
        }
    }
}
