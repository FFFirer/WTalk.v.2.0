using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace WTalk.Helpers
{
    public class TCPHelper
    {
        public IPAddress IP { get; set; }
        public int port { get; set; }
        public TcpClient tcpClient { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }

        public TCPHelper()
        {
            port = 51888;
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(var ip in ips)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = ip;
                    break;
                }
            }
        }

        //指定IP地址
        public TCPHelper(IPAddress ip, int port)
        {
            this.IP = ip;
            this.port = port;
        }
        public void ConnectServer()
        {
            try
            {
                tcpClient = new TcpClient(IP.ToString(), port);
                NetworkStream stream = tcpClient.GetStream();
                br = new BinaryReader(stream);
                bw = new BinaryWriter(stream);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void Send2Server(string msg)
        {
            if (bw != null)
            {
                try
                {
                    bw.Write(msg);
                    bw.Flush();
                }
                catch(Exception e)
                {
                    throw e;//异常处理
                }
            }
        }
    }
}
