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
using NAudio;

namespace Communicator_LAN
{
    public partial class Main_window : Form
    {
        private TcpListener publicListener = null;
        NAudio.Wave.WaveOut _wo = new NAudio.Wave.WaveOut();
        int RATE = 22100;
        int BUFFERSIZE = (int)Math.Pow(2, 12);

        private bool consoleDebug = true;

        private string ServerName;
        private string Password;
        private int MaxUsers;
        private Form parent;
        private Size delta;
        private Thread incomingUsersThread;
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
            if (consoleDebug) Console.WriteLine("");
            if (consoleDebug) Console.WriteLine(" *** NASTĄPIŁA ZMIANA W LIŚCIE KLIENTÓW *** ");
            if (consoleDebug) Console.WriteLine("");
            foreach (Client recipient in currentClientList)
            {
                Thread sendChanges = new Thread(() =>
                {
                    TcpListener listener = null;
                    TcpClient client = null;
                    NetworkStream stream = null;
                    BinaryWriter writer = null;

                    client = new TcpClient();
                    if (consoleDebug) Console.WriteLine("sendChanges: Łączę z klientem " + recipient.getIP() + ":" + recipient.getPort());
                    if (consoleDebug) Console.WriteLine("...");
                    try
                    {
                        Random r = new Random();
                        Thread.Sleep(r.Next(1, 3000));
                        client.ConnectAsync(recipient.getIP(), recipient.getPort()).Wait(2000);
                        if (client.Connected)
                        {
                            if (consoleDebug) Console.WriteLine("sendChanges: Połączono z " + recipient.getIP() + ":" + recipient.getPort());
                            stream = client.GetStream();
                            writer = new BinaryWriter(stream);
                            writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                                COMMUNICATION_VALUES.SENDING.REFRESH_YOUR_LIST);
                            if (consoleDebug) Console.WriteLine("sendChanges: Wysyłam polecenie odświeżenia listy dla klienta " + client.Client.RemoteEndPoint.ToString());
                        }
                        else
                        {
                            if (consoleDebug) Console.WriteLine("sendChanges: Nie udało się nawiązać połączenia z " + recipient.getIP() + ":" + recipient.getPort());
                        }
                    } catch (SocketException socketex)
                    {
                        if (consoleDebug) Console.WriteLine("sendChanges: Nie udało się nawiązać połączenia z " + recipient.getIP());
                        if (consoleDebug) Console.WriteLine("     " + socketex.Message);
                    }
                });
                sendChanges.IsBackground = true;
                sendChanges.Start();
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
            Text = ServerName + " - Serwer";
        }

        bool first = true;
        private static Color defaultBackgroundColor;
        private void Main_window_Shown(object sender, EventArgs e)
        {
            if (first)
            {
                takeDataAndCloseParentForm(parent);
            }
            try
            {
                incomingUsersThread = new Thread(() =>
                {
                    TcpClient client = null;
                    NetworkStream stream = null;
                    BinaryWriter writer = null;
                    BinaryReader reader = null;

                    byte[] IP = stringToByte(IP_textBox.Text);
                    publicListener = new TcpListener(new System.Net.IPEndPoint(new System.Net.IPAddress(IP), 45000));
                    publicListener.Start();
                    if (consoleDebug) Console.WriteLine("Tworzenie listenera dla " + IP_textBox.Text);
                    while (true)
                    {
                        if (consoleDebug) Console.WriteLine("Oczekiwanie na klienta...");
                        using (client = publicListener.AcceptTcpClient())
                        {
                            if (consoleDebug) Console.WriteLine("Przyjęto wezwanie od " + client.Client.RemoteEndPoint.ToString());
                            using (stream = client.GetStream())
                            {
                                try
                                {
                                    reader = new BinaryReader(stream);
                                    string received = reader.ReadString();
                                    if (consoleDebug) Console.WriteLine("Otrzymano wiadomość:");
                                    if (consoleDebug) Console.WriteLine("      " + received);
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
                                                    if (consoleDebug) Console.WriteLine("Logowanie klienta " + client.Client.RemoteEndPoint.ToString());
                                                    string username = received.Substring(received.IndexOf('|') + 1, received.LastIndexOf('|') - 1 - received.IndexOf('|'));
                                                    writer = new BinaryWriter(stream);
                                                    User incomingClient = createClient(username, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                                                    Invoke(new MethodInvoker(() =>
                                                    {
                                                        addClient(incomingClient);
                                                    }));
                                                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.SERVER_NAME + ServerName +
                                                        "|" + incomingClient.GetClient().getPort().ToString());
                                                }
                                                else if (currentClientList.Count + 1 >= MaxUsers)
                                                {
                                                    writer = new BinaryWriter(stream);
                                                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.SERVER_FULL);
                                                    if (consoleDebug) Console.WriteLine("Wysyłanie wiadomości do " +
                                                        client.Client.RemoteEndPoint.ToString() +
                                                        " - Serwer pełny");
                                                }
                                                else
                                                {
                                                    writer = new BinaryWriter(stream);
                                                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.PASSWORD_INCORRECT);
                                                    if (consoleDebug) Console.WriteLine("Wysyłanie wiadomości do " +
                                                        client.Client.RemoteEndPoint.ToString() +
                                                        " - Złe hasło.");
                                                }
                                                break;
                                            }
                                            case COMMUNICATION_VALUES.RECEIVING.SEND_ME_CLIENT:
                                            {
                                                int number = Convert.ToInt32(received.Substring(received.LastIndexOf('|') + 1));
                                                if (consoleDebug) Console.WriteLine("Klient " + client.Client.RemoteEndPoint.ToString() + " żąda " + number + " klienta na obecnej liście.");
                                                if (number < currentClientList.Count)
                                                {
                                                    Client _tosend = currentClientList[number];
                                                    writer = new BinaryWriter(stream);
                                                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                                                        COMMUNICATION_VALUES.SENDING.NEXT_CLIENT_FROM_LIST +
                                                        _tosend.getUsername() + "|" + _tosend.getIP());
                                                    if (consoleDebug) Console.WriteLine("Wysyłam pozycję " + number);
                                                }
                                                else
                                                {
                                                    writer = new BinaryWriter(stream);
                                                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                                                        COMMUNICATION_VALUES.SENDING.END_OF_LIST);
                                                    if (consoleDebug) Console.WriteLine("Nie ma takiej pozycji na liście, wysyłam informację.");
                                                }
                                                break;
                                            }
                                            case COMMUNICATION_VALUES.RECEIVING.TALKING:
                                            {
                                                string username = received.Substring(received.LastIndexOf('|') + 1);
                                                if (consoleDebug) Console.WriteLine("Klient " + client.Client.RemoteEndPoint.ToString() + " mówi.");
                                                foreach (Control c in UsersPanel.Controls)
                                                {
                                                    if (c.GetType() == typeof(User))
                                                    {
                                                        if ((c as User).Username.Text == username)
                                                        {
                                                            Invoke(new MethodInvoker(() =>
                                                            {
                                                                defaultBackgroundColor = (c as User).BackColor;
                                                                (c as User).BackColor = Color.LightBlue;
                                                                (c as User).isTalking = true;
                                                            }));
                                                            Thread receive = new Thread(() =>
                                                            {
                                                                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, (c as User).GetClient().getPort() + 1000);
                                                                UdpClient udpc = new UdpClient();
                                                                try
                                                                {
                                                                    udpc = new UdpClient(ipep);
                                                                }
                                                                catch { }
                                                                bool talking = (c as User).isTalking;
                                                                while (talking)
                                                                {
                                                                    byte[] data = new byte[BUFFERSIZE];
                                                                    data = udpc.Receive(ref ipep);
                                                                    int x = 1;
                                                                    /* odtworzyc dla testu */

                                                                    Invoke(new MethodInvoker(() =>
                                                                    {
                                                                        talking = (c as User).isTalking;
                                                                        NAudio.Wave.IWaveProvider prov = new NAudio.Wave.RawSourceWaveStream(new MemoryStream(data), new NAudio.Wave.WaveFormat(RATE, 1));

                                                                        _wo.Init(prov);
                                                                        _wo.Play();
                                                                    }));
                                                                }
                                                                udpc.Close();
                                                                udpc.Dispose();
                                                            })
                                                            { IsBackground = true };
                                                            receive.Start();


                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                            case COMMUNICATION_VALUES.RECEIVING.NOT_TALKING:
                                            {
                                                string username = received.Substring(received.LastIndexOf('|') + 1);
                                                if (consoleDebug) Console.WriteLine("Klient " + client.Client.RemoteEndPoint.ToString() + " przestał mówić.");
                                                foreach (Control c in UsersPanel.Controls)
                                                {
                                                    if (c.GetType() == typeof(User))
                                                    {
                                                        if ((c as User).Username.Text == username)
                                                        {
                                                            Invoke(new MethodInvoker(() =>
                                                            {
                                                                (c as User).BackColor = defaultBackgroundColor;
                                                                (c as User).isTalking = false;
                                                                UdpClient ud = new UdpClient();
                                                                ud.Connect(IP_textBox.Text, (c as User).GetClient().getPort() + 1000);
                                                                ud.Send(new byte[1] { 0 }, 1);
                                                                _wo.Stop();
                                                            }));
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                            case COMMUNICATION_VALUES.RECEIVING.I_AM_DISCONNECTING:
                                            {
                                                string username = received.Substring(received.LastIndexOf('|') + 1);
                                                foreach(Control c in UsersPanel.Controls)
                                                {
                                                    if (c.GetType() == typeof(User))
                                                    {
                                                        Invoke(new MethodInvoker(() => {
                                                            currentClientList.Remove((c as User).GetClient());
                                                            UsersPanel.Controls.Remove((c as User));
                                                        }));
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    else if (header == COMMUNICATION_VALUES.CONNECTION_SERVER)
                                    {
                                        publicListener.Stop();
                                        break;
                                    }
                                }
                                catch (IOException ioex)
                                {
                                    if (consoleDebug) Console.Write(ioex.Message);
                                    if (consoleDebug) Console.Write(ioex.StackTrace);
                                }
                            }
                        }
                    }

                })
                {
                    IsBackground = true
                };
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
            int port = 45001;
            if (currentClientList.Count != 0)
            {
                int[] ports = new int[currentClientList.Count];
                int i = 0;
                foreach (Client c in currentClientList)
                {
                    ports[i] = c.getPort();
                    i++;
                }
                Array.Sort(ports);
                foreach (int p in ports)
                {
                    if (p == port)
                        port++;
                    else
                        break;
                }
            }
            User u = new User(name, address, port);
            if (UsersPanel.VerticalScroll.Visible)
                u.Width = UsersPanel.Width - 20;
            else
                u.Width = UsersPanel.Width - 5;
            u.Location = new Point(u.Location.X, 27 * UsersPanel.Controls.Count);
            u.Username.Text = name;
            u.Kick_button.Click += new EventHandler(delegate (object o, EventArgs e)
            {
                Thread kickThread = new Thread(() =>
                {
                    TcpClient c = new TcpClient();
                    c.ConnectAsync(u.GetClient().getIP(), u.GetClient().getPort()).Wait(500);
                    if (c.Connected)
                    {
                        NetworkStream ns = c.GetStream();
                        BinaryWriter bw = new BinaryWriter(ns);
                        bw.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.YOU_HAVE_BEEN_KICKED);
                        c.Close();
                        c.Dispose();
                        ns.Dispose();
                        bw.Dispose();
                    }
                })
                { IsBackground = true };
                kickThread.Start();
                currentClientList.Remove(u.GetClient());
                UsersPanel.Controls.Remove(u); 
            });

            u.Mute_button.Click += new EventHandler(delegate (object o, EventArgs e)
            {
                if (u.BackColor != Color.LightSalmon)
                {
                    u.isMuted = true;
                    defaultBackgroundColor = u.BackColor;
                    u.BackColor = Color.LightSalmon;


                    Thread muteThread = new Thread(() =>
                    {
                        TcpClient c = new TcpClient();
                        c.ConnectAsync(u.GetClient().getIP(), u.GetClient().getPort()).Wait(500);
                        if (c.Connected)
                        {
                            NetworkStream ns = c.GetStream();
                            BinaryWriter bw = new BinaryWriter(ns);
                            bw.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.YOU_HAVE_BEEN_MUTED);
                            c.Close();
                            c.Dispose();
                            ns.Dispose();
                            bw.Dispose();
                        }
                    })
                    { IsBackground = true };
                    muteThread.Start();
                }
                else
                {
                    u.isMuted = false;
                    u.BackColor = defaultBackgroundColor;

                    Thread muteThread = new Thread(() =>
                    {
                        TcpClient c = new TcpClient();
                        c.ConnectAsync(u.GetClient().getIP(), u.GetClient().getPort()).Wait(500);
                        if (c.Connected)
                        {
                            NetworkStream ns = c.GetStream();
                            BinaryWriter bw = new BinaryWriter(ns);
                            bw.Write(COMMUNICATION_VALUES.CONNECTION_SERVER + COMMUNICATION_VALUES.SENDING.YOU_HAVE_BEEN_UNMUTED);
                            c.Close();
                            c.Dispose();
                            ns.Dispose();
                            bw.Dispose();
                        }
                    })
                    { IsBackground = true };
                    muteThread.Start();
                }
            });
            return u;
        }

        private void addClient(User u)
        {
            UsersPanel.Controls.Add(u);
            if (UsersPanel.HorizontalScroll.Visible)
                Main_window_Resize(this, null);
            currentClientList.Add(u.GetClient());
        }

        private void SendCloseRequest(Client c)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            BinaryWriter writer = null;

            client = new TcpClient();
            if (consoleDebug) Console.WriteLine("shutdownInfo: Łączę z klientem " + c.getIP() + ":" + c.getPort());
            if (consoleDebug) Console.WriteLine("...");
            try
            {
                Random r = new Random();
                Thread.Sleep(r.Next(1, 3000));
                client.ConnectAsync(c.getIP(), c.getPort()).Wait(2000);
                if (client.Connected)
                {
                    if (consoleDebug) Console.WriteLine("shutdownInfo: Połączono z " + c.getIP() + ":" + c.getPort());
                    stream = client.GetStream();
                    writer = new BinaryWriter(stream);
                    writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER +
                        COMMUNICATION_VALUES.SENDING.SERVER_SHUT_DOWN);
                    if (consoleDebug) Console.WriteLine("shutdownInfo: Wysyłam informację o zamknięciu serwera dla klienta " + client.Client.RemoteEndPoint.ToString());
                }
                else
                {
                    if (consoleDebug) Console.WriteLine("shutdownInfo: Nie udało się nawiązać połączenia z " + c.getIP() + ":" + c.getPort());
                }
            }
            catch (SocketException socketex)
            {
                if (consoleDebug) Console.WriteLine("shutdownInfo: Nie udało się nawiązać połączenia z " + c.getIP() + ":" + c.getPort());
                if (consoleDebug) Console.WriteLine("     " + socketex.Message);
            }
        }

        private void Main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentClientList.Count != 0)
            {
                if (MessageBox.Show("Serwer jest w trakcie pracy. Czy na pewno chcesz go zamknąć?", "Wyłączenie serwera",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int i = 0;
                    foreach (Client c in currentClientList)
                    {
                        SendCloseRequest(c);
                    }
                }
            }
            forceCloseIncomingThread();
            publicListener.Stop();
            parent.Show();
        }

        private void forceCloseIncomingThread()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            BinaryWriter writer = null;

            client = new TcpClient();
            client.ConnectAsync(IP_textBox.Text, 45000).Wait(100);
            stream = client.GetStream();
            writer = new BinaryWriter(stream);
            writer.Write(COMMUNICATION_VALUES.CONNECTION_SERVER);
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
