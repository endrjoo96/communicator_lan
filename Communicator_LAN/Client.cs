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
        private bool isMuted=false, isListening=false, isTalking=false;

        public Client(string ip, string username)
        {
            IP = ip;
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
    }
}
