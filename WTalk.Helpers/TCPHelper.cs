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
        public TcpListener tcpListener { get; set; }
        public TcpClient tcpClient { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }
        public event EventHandler<string> DataHandler;  //接受数据处理
        public event EventHandler<TcpClient> AddTcpClientHandler;   //添加客户端处理
        public event EventHandler<string> ExHandler;  //丢失连接处理

        //指定本机
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

        //连接服务器
        public void Ready()
        {
            try
            {
                if(tcpClient == null)
                {
                    tcpClient = new TcpClient(IP.ToString(), port);
                }
                NetworkStream stream = tcpClient.GetStream();
                br = new BinaryReader(stream);
                bw = new BinaryWriter(stream);
            }
            catch(Exception e)
            {
                throw e;

            }
        }
        //发送消息
        public void SendMessage(string msg)
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
                    //throw e;//异常处理
                    if(ExHandler != null)
                    {
                        ExHandler(null, string.Format("{0}:{1}-->发送失败-->{2}", IP.ToString(), port, e.Message));
                    }
                    return;

                }
            }
        }
        //接收数据
        public void ReceiveData()
        {
            string receiveString = null;
            while (true)
            {
                try { receiveString = br.ReadString(); }
                catch
                {
                    //throw e;
                    if(ExHandler != null)
                    {
                        ExHandler(null, string.Format("{0}:{1}-->已经掉线", IP.ToString(), port));
                    }
                    return;
                }
                if(DataHandler != null && receiveString != null)
                {
                    DataHandler(bw, receiveString);
                }
            }
        }
        //监听客户端
        public void ListenClient()
        {
            TcpClient newClient = null;
            while(true)
            {
                try
                {
                    newClient = tcpListener.AcceptTcpClient();
                    if(AddTcpClientHandler!= null)
                    {
                        AddTcpClientHandler(null, newClient);
                    }
                }
                catch { break; }
            }
            
        }
        //开始监听
        public void StartListen(int p)
        {
            port = p;
            tcpListener = new TcpListener(IP, p);
            tcpListener.Start();
        }
        //停止监听
        public void StopListen()
        {
            if(tcpListener != null)
            {
                tcpListener.Stop();
            }
        }
    }
}
