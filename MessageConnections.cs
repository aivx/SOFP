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
    public partial class MessageConnections : Form
    {
        public MessageConnections()
        {
            InitializeComponent();
        }

        private void MessageConnections_Load(object sender, EventArgs e)
        {
            
        }

        private void MessageConnections_Shown(object sender, EventArgs e)
        {
            if (Connection.openConnections())
            {
                DataUpdate.updates();
            }
            this.Close();
        }
    }
}
