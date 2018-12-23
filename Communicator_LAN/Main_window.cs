using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communicator_LAN
{
    public partial class Main_window : Form
    {
        private string ServerName;
        private string Password;
        private int MaxUsers;
        private Form parent;
        private Size delta;
        private Thread x;

        public Main_window(Form parent)
        {
            InitializeComponent();
            this.parent = parent;
            IP_textBox.Text = getMachineIP();

            delta = new Size(Width - UsersPanel.Width, Height - UsersPanel.Height);
        }

        private string getMachineIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        private void takeDataAndCloseParentForm(Form parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c.Name)
                {
                    case VALID_FIELDS.SERVER_NAME:
                    {
                        ServerName = c.Text;
                        break;
                    }
                    case VALID_FIELDS.SERVER_PASSWORD:
                    {
                        Password = c.Text;
                        break;
                    }
                    case VALID_FIELDS.SERVER_MAXUSERS:
                    {
                        MaxUsers = Convert.ToInt32((c as NumericUpDown).Value);
                        break;
                    }
                }
            }
            parent.Hide();
            Text = ServerName+" - Serwer";
        }

        private void Main_window_Shown(object sender, EventArgs e)
        {
            takeDataAndCloseParentForm(parent);
            for (int i=0; i<7; i++){
                User u = new User();
                if (UsersPanel.VerticalScroll.Visible)
                    u.Width = UsersPanel.Width - 20;
                else
                    u.Width = UsersPanel.Width - 5;
                u.Location = new Point(u.Location.X, 27 * i);

                u.Username.Text = "Generated user " + (i+1);

                UsersPanel.Controls.Add(u);
            }
            User a = createClient("pomidorek3000");
            addClient(a);
            x = new Thread(() =>
            {
                TcpListener listener = null;
                TcpClient client = null;
                NetworkStream stream = null;
                BinaryWriter writer = null;
                BinaryReader reader = null;

                //przerzucić kod z tcpserver to watka, moze zatrybi

                //TCPServer server = new TCPServer(this);
                //server.setConnection(IP_textBox.Text, 65505);
                byte[] IP = stringToByte(IP_textBox.Text);
                listener = new TcpListener(new System.Net.IPEndPoint(new System.Net.IPAddress(IP), 65505));
                listener.Start();
                while (true)
                {
                    Console.WriteLine("czekam...");
                    using (client = listener.AcceptTcpClient())
                    {
                        Console.WriteLine("coś tu do nas weszło");
                        Invoke(new MethodInvoker(() =>
                        {
                            User incomingClient = createClient("XD");
                            addClient(incomingClient);
                        }));
                    }
                }
            });
            x.IsBackground = true;
            x.Start();
        }

        private byte[] stringToByte(string IP)
        {
            IPAddress address = IPAddress.Parse(IP);
            byte[] bytes = address.GetAddressBytes();
            return bytes;
        }

        public User createClient(string name)
        {
            User u = new User();
            if (UsersPanel.VerticalScroll.Visible)
                u.Width = UsersPanel.Width - 20;
            else
                u.Width = UsersPanel.Width - 5;
            u.Location = new Point(u.Location.X, 27 * UsersPanel.Controls.Count);
            u.Username.Text = name;
            return u;
        }

        private void addClient(User u)
        {
            UsersPanel.Controls.Add(u);
            if (UsersPanel.HorizontalScroll.Visible)
                Main_window_Resize(this, null);
        }

        private void Main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Close();
        }

        private void Main_window_Resize(object sender, EventArgs e)
        {
            UsersPanel.Size = new Size(Width - delta.Width, Height - delta.Height);
            foreach(Control c in UsersPanel.Controls)
            {
                if (c.GetType() == typeof(User))
                {
                    if(UsersPanel.VerticalScroll.Visible)
                        c.Width = UsersPanel.Width - 20;
                    else
                        c.Width = UsersPanel.Width - 5;
                }
            }
        }
    }
}
