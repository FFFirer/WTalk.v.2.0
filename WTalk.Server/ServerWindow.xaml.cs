using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using WTalk.Helpers;
using WTalk.Domain;

namespace WTalk.Server
{
    /// <summary>
    /// ServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        TCPHelper helper = null;
        private List<UserOnServer> users = new List<UserOnServer>();
        public List<IP> IPList;
        public ServerWindow()
        {
            InitializeComponent();
            IPList = new List<IP>();
            GetAllIPs();
            cbbIP.ItemsSource = IPList;
            //DataHelpers.ShowHandler += ShowMsg;   //展示消息日志
            CC.DataHandle.ShowOnServerWindow += ShowMsg;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            string ip = this.cbbIP.SelectedValue.ToString();
            //MessageBox.Show(ip+":"+txtPort.Text+"已开始监听");
            int p = int.Parse(txtPort.Text);
            helper = new TCPHelper(IPAddress.Parse(ip), p);
            helper.ExHandler += ShowMsg;
            helper.StartListen(p);
            ShowMsg(null, string.Format("{0}:{1}-->监听已开始", helper.IP, helper.port));
            helper.AddTcpClientHandler += CreateClientTask;
            Task.Run(() => helper.ListenClient());
        }

        //将消息写入消息框
        public void ShowMsg(object sender, string data)
        {
            string time = DateTime.Now.ToLongTimeString();
            string msg = string.Format("[{0}]-->{1}\n", time, data);
            this.txtbMsg.Dispatcher.Invoke(() => this.txtbMsg.Text += msg);
        }

        //为每个用户创建单独线程
        public void CreateClientTask(object sender, TcpClient client)
        {
            ShowMsg(null, string.Format("{0}-->连接成功", client.Client.RemoteEndPoint.ToString()));
            //
            //ServerUser user = new ServerUser(client);
            UserOnServer user = new UserOnServer(client);
            user.helper.ExHandler += ShowMsg;
            users.Add(user);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            helper.StopListen();
            ShowMsg(null, "监听已停止");
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void GetAllIPs()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach(var a in adapters)
            {
                IPInterfaceProperties ip = a.GetIPProperties();
                if(ip.UnicastAddresses.Count>0)
                {
                    foreach(IPAddressInformation add in ip.UnicastAddresses)
                    {
                        if(add.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IP i = new IP(add.Address.ToString(), a.Name);
                            IPList.Add(i);
                        }
                    }
                }
                
            }
            cbbIP.SelectedIndex = 0;
        }
        
    }

    public class IP
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public IP(string address, string name)
        {
            this.Address = address;
            this.Name = string.Format("{0}--{1}", address, name);
        }
    }
}
