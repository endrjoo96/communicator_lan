using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communicator_LAN
{
    public partial class User : UserControl
    {
        public User()
        {
            InitializeComponent();
        }

        public void User_Resize(object sender, EventArgs e)
        {
            Kick_button.Location = new Point(Width - 47, Kick_button.Location.Y);
            Mute_button.Location = new Point(Width - 93, Mute_button.Location.Y);
        }
    }
}
