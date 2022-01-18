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
    public partial class NewConnect : Form
    {
        public NewConnect()
        {
            InitializeComponent();
            password.PasswordChar = '\u25CF';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connection.newConnect(source.Text, database.Text, login.Text, password.Text);
            MessageConnections ms = new MessageConnections();
            ms.Show();
            this.Close();
        }

        private void NewConnect_Load(object sender, EventArgs e)
        {
            Connection.closeConnections();
        }
    }
}
