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

        Form nc, cu, or, pr, ul;

        private void ShowNewForm(object sender, EventArgs e)
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<NewConnect>())
            {
                nc = new NewConnect();
                nc.MdiParent = this;
                nc.WindowState = FormWindowState.Normal;
                nc.Show();
            }
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

        private void openCustomers()
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<Customers>())
            {
                cu = new Customers();
                cu.MdiParent = this;
                cu.Show();
            }
        }

        private void покупателиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCustomers();
            cu.WindowState = FormWindowState.Maximized;
        }

        private void openOrders()
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<Orders>())
            {
                or = new Orders();
                or.MdiParent = this;
                or.Show();
            }
        }

        private void сделкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openOrders();
            or.WindowState = FormWindowState.Maximized;
        }

        private void openProducts()
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<Products>())
            {
                pr = new Products();
                pr.MdiParent = this;
                pr.Show();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<AboutBox1>())
            {
                AboutBox1 about = new AboutBox1();
                about.MdiParent = this;
                about.Show();
            }
        }

        private void товарыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openProducts();
            pr.WindowState = FormWindowState.Maximized;
        }

        private void openLoginUsers()
        {
            bool IsFormOpened<TForm>() where TForm : Form
            {
                return Application.OpenForms.OfType<TForm>().Any();
            }
            if (!IsFormOpened<LoginUsers>())
            {
                pr = new LoginUsers();
                pr.MdiParent = this;
                pr.Show();
            }
        }

        private void пользователиАвторизацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLoginUsers();
            pr.WindowState = FormWindowState.Maximized;
        }

        private void открытьВсеОкнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCustomers();
            openOrders();
            openProducts();
            openLoginUsers();
            LayoutMdi(MdiLayout.Cascade);
        }
    }
}
