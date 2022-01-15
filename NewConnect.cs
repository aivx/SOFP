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
            if(Connection.newconnect(source.Text, login.Text, password.Text))
            {
                this.Close();
            }  
        }
    }
}
