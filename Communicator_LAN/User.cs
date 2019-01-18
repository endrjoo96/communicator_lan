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
        Client client;
        public bool isTalking = false, isMuted = false;
        public User(string username, string ip, int port)
        {
            InitializeComponent();
            client = new Client(ip, port, username);
        }

        public void User_Resize(object sender, EventArgs e)
        {
            Kick_button.Location = new Point(Width - 47, Kick_button.Location.Y);
            Mute_button.Location = new Point(Width - 93, Mute_button.Location.Y);
        }

        internal Client GetClient()
        {
            return client;
        }
    }
}
