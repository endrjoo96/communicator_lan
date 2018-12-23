using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Communicator_LAN
{
    class TCPServer
    {
        TcpListener listener = null;
        TcpClient client = null;
        NetworkStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;
        Main_window parent;
        public TCPServer(Form caller)
        {
            if(caller.GetType() == typeof(Main_window))
                parent = caller as Main_window;
        }

        public async void setConnection(string ip, int port)
        {
            byte[] IP = stringToByte(ip);
            listener = new TcpListener(new System.Net.IPEndPoint(new System.Net.IPAddress(IP), port));
            listener.Start();
            while (true)
            {
                using(client = listener.AcceptTcpClient())
                {
                    parent.createClient("XD");
                }
            }
        }

        private byte[] stringToByte(string IP)
        {
            IPAddress address = IPAddress.Parse(IP);
            byte[] bytes = address.GetAddressBytes();
            return bytes;
        }
    }
}
