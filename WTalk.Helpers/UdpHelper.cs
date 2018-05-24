using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;

namespace WTalk.Helpers
{
    //UDP发送模式
    public class UdpHelper
    {
        public IPAddress localAddress { get; set; }
        public IPEndPoint localEndPoint { get; set; }
        public UdpClient udpClient { get; set; }
        //事件
        public event EventHandler<string> HandleData;   //数据处理

        //UDP监听端口51666
        public UdpHelper()
        {
            if(localEndPoint!=null)
            {
                udpClient = new UdpClient(localEndPoint);
            }
        }

        //异步发送
        public async void SendMessage(object sender, )

        //异步接收
        public async void ReceiveUdpDataAsync()
        {
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                string s = Encoding.Unicode.GetString(result.Buffer);
                if(HandleData != null)
                {
                    HandleData(null, s);
                }
            }
        }

    }
}
