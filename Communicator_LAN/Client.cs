using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicator_LAN
{
    class Client
    {
        private string IP, Username;
        private int Port;
        private bool isMuted=false, isListening=false, isTalking=false;

        public Client(string ip, int port, string username)
        {
            IP = ip;
            Port = port;
            Username = username;
        }

        public string getIP()
        {
            return IP;
        }

        public string getUsername()
        {
            return Username;
        }

        public int getPort()
        {
            return Port;
        }
    }
}
