using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Helpers;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace WTalk.Domain
{
    public class ServerUser
    {
        public TCPHelper Helper;
        public string UserID;
        public ServerUser(TcpClient tcpClient)
        {
            Helper = new TCPHelper();
            Helper.tcpClient = tcpClient;
            Helper.DataHandler += DataHelpers.Handle4Server;
            Helper.Ready();
            Task.Run(() => Helper.ReceiveData());
        }
    }
}
