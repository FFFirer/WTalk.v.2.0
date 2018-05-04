using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using WTalk.Helpers;

namespace WTalk.Server
{
    public class UserOnServer
    {
        public TCPHelper helper;
        public string UserId;
        public UserOnServer(TcpClient tcpClient)
        {
            helper = new TCPHelper();
            helper.tcpClient = tcpClient;
            helper.DataHandler += CC.DataHandle.Handle;
            helper.Ready();
            Task.Run(() => helper.ReceiveData());
        }
        
    }
}
