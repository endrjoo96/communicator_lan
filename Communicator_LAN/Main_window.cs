using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Thread incomingUsersThread;
        private Thread listeningThread;     //wątek do przeszukiwania listy w poszukiwaniu osób, które aktualnie coś mówią.
        private ObservableCollection<Client> currentClientList;

        public Main_window(Form parent)
        {
            InitializeComponent();
            this.parent = parent;
            IP_textBox.Text = getMachineIP();

            currentClientList = new ObservableCollection<Client>();
            delta = new Size(Width - UsersPanel.Width, Height - UsersPanel.Height);

            currentClientList.CollectionChanged += CurrentClientList_CollectionChanged;
        }

        private void CurrentClientList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Client recipient in currentClientList)
            {
                Thread sendChanges = new Thread(() =>
                {
                    TcpListener listener = null;
                    TcpClient client = null;
                    NetworkStream stream = null;
                    BinaryWriter writer = null;

                    client = new TcpClient();
                    client.Connect(recipient.getIP(), 65505);
                    stream = client.GetStream();
                    writer = new BinaryWriter(stream);
                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER+
                        COMMUNICATION_VALUES.SENDING.REFRESH_YOUR_LIST);
                });
            }
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

        bool first = true;
        private static Color defaultBackgroundColor;
        private void Main_window_Shown(object sender, EventArgs e)
        {
            if (first)
            {
                takeDataAndCloseParentForm(parent);
                for (int i = 0; i < 3; i++)
                {
                    User u = createClient("Generated user " + (i + 1), "127.0.0." + (i + 1));
                    addClient(u);
                }
                User a = createClient("pomidorek3000", "127.0.0.254");
                addClient(a);
                first = false;
            }
            try
            {
                incomingUsersThread = new Thread(() =>
                {
                    TcpListener listener = null;
                    TcpClient client = null;
                    NetworkStream stream = null;
                    BinaryWriter writer = null;
                    BinaryReader reader = null;

                    byte[] IP = stringToByte(IP_textBox.Text);
                    listener = new TcpListener(new System.Net.IPEndPoint(new System.Net.IPAddress(IP), 65505));
                    listener.Start();
                    while (true)
                    {
                        Console.WriteLine("czekam...");
                        using (client = listener.AcceptTcpClient())
                        {
                            Console.WriteLine("coś tu do nas weszło");
                            using (stream = client.GetStream())
                            {
                                reader = new BinaryReader(stream);
                                string received = reader.ReadString();
                                string header = received.Substring(0, received.IndexOf(':') + 1);
                                if (header == COMMUNICATION_VALUES.CONNECTION_CLIENT)
                                {
                                    string reason = received.Substring(received.IndexOf(':') + 1, (received.IndexOf('|')) - received.IndexOf(':'));
                                    switch (reason)
                                    {
                                        case COMMUNICATION_VALUES.RECEIVING.USERNAME_AND_PASSWORD:
                                        {
                                            string password = received.Substring(received.LastIndexOf('|') + 1);
                                            if (password == Password && currentClientList.Count + 1 < MaxUsers)
                                            {
                                                string username = received.Substring(received.IndexOf('|') + 1, received.LastIndexOf('|') - 1 - received.IndexOf('|'));
                                                writer = new BinaryWriter(stream);
                                                writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.SERVER_NAME + ServerName);
                                                Invoke(new MethodInvoker(() =>
                                                {
                                                    User incomingClient = createClient(username, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                                                    addClient(incomingClient);
                                                }));
                                            }
                                            else if (currentClientList.Count + 1 >= MaxUsers)
                                            {
                                                writer = new BinaryWriter(stream);
                                                writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.SERVER_FULL);
                                            }
                                            else
                                            {
                                                writer = new BinaryWriter(stream);
                                                writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.PASSWORD_INCORRECT);
                                            }
                                            break;
                                        }
                                        case COMMUNICATION_VALUES.RECEIVING.SEND_ME_CLIENT:
                                        {
                                            int number = Convert.ToInt32(received.Substring(received.LastIndexOf('|') + 1));
                                            if (number < currentClientList.Count)
                                            {
                                                Client _tosend = currentClientList[number];
                                                writer = new BinaryWriter(stream);
                                                writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                                                    COMMUNICATION_VALUES.SENDING.NEXT_CLIENT_FROM_LIST +
                                                    _tosend.getUsername() + "|" + _tosend.getIP());
                                                int x = 0;
                                            }
                                            else
                                            {
                                                writer = new BinaryWriter(stream);
                                                writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                                                    COMMUNICATION_VALUES.SENDING.END_OF_LIST);
                                                int x = 0;
                                            }
                                            break;
                                        }
                                        case COMMUNICATION_VALUES.RECEIVING.TALKING:
                                        {
                                            string username = received.Substring(received.LastIndexOf('|') + 1);
                                            foreach (Control c in UsersPanel.Controls)
                                            {
                                                if (c.GetType() == typeof(User))
                                                {
                                                    if((c as User).Username.Text == username)
                                                    {
                                                        Invoke(new MethodInvoker(() =>
                                                        {
                                                            defaultBackgroundColor = (c as User).BackColor;
                                                            (c as User).BackColor = Color.LightBlue;
                                                        }));
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case COMMUNICATION_VALUES.RECEIVING.NOT_TALKING:
                                        {
                                            string username = received.Substring(received.LastIndexOf('|') + 1);
                                            foreach (Control c in UsersPanel.Controls)
                                            {
                                                if (c.GetType() == typeof(User))
                                                {
                                                    if ((c as User).Username.Text == username)
                                                    {
                                                        Invoke(new MethodInvoker(() =>
                                                        {
                                                            (c as User).BackColor = defaultBackgroundColor;
                                                        }));
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                });
                incomingUsersThread.IsBackground = true;
                incomingUsersThread.Start();
            }
            catch (IOException ex) {
                Console.WriteLine(ex.Message);
                Console.Write(ex.StackTrace);
                Main_window_Shown(this, null);
            }
        }

        private byte[] stringToByte(string IP)
        {
            IPAddress address = IPAddress.Parse(IP);
            byte[] bytes = address.GetAddressBytes();
            return bytes;
        }

        public User createClient(string name, string address)
        {
            User u = new User(name, address);
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
            currentClientList.Add(u.GetClient());
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
