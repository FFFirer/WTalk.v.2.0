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
        public List<UserOnServer> users = new List<UserOnServer>();
        public List<IP> IPList;
        public ServerWindow()
        {
            InitializeComponent();
            IPList = new List<IP>();
            GetAllIPs();
            cbbIP.ItemsSource = IPList;
            //DataHelpers.ShowHandler += ShowMsg;   //展示消息日志
            CC.DataHandle.ShowOnServerWindow += ShowMsg;
            CC.DataHandle.LoginHandler += Register; 
            CC.DataHandle.UpdateFriendListHandler += Update; //向在线用户更新好友列表
            CC.DataHandle.AddHandler += Send2User;
            CC.DataHandle.TalkHandler += Talk2User;
            CC.ServerHandle.PresenceHandler += Presence2All;
            TCPHelper.ExitHandler += RemoveUserFormList;
            this.Closing += ServerWindow_Closing;
            CC.DataHandle.RemoveFriendHandler += DataHandle_RemoveFriendHandler; ;    //删除好友事件
        }

        private void Presence2All(object sender, string e)
        {
            List<User> usersOnline = (List<User>)sender;
            foreach(var a in usersOnline)
            {
                foreach(var s in users)
                {
                    if(s.UserId == a.UserId)
                    {
                        s.helper.SendMessage(e);
                    }
                }
            }
        }

        private void DataHandle_RemoveFriendHandler(object sender, RemoveContract e)
        {
            var user = users.Where(p => p.UserId.Equals(e.FriendId)).FirstOrDefault();
            if(user != null)
            {
                user.helper.SendMessage(string.Format("REMOVE@{0}", DataHelpers.XMLSer<RemoveContract>(e)));
            }
        }

        private void ServerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            users.Clear();
            CC.DataHandle.ClearPresence();
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
            users.Add(user);
            user.helper.ExHandler += ShowMsg;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            helper.StopListen();
            ShowMsg(null, "监听已停止");
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            CC.DataHandle.ClearPresence();
        }

        public void RemoveUserFormList(object sender, string ip)
        {
            var u = users.Where(p => p.helper.tcpClient.Client.RemoteEndPoint.ToString().Equals(ip)).FirstOrDefault();
            users.Remove(u);
            //向所有在线好友发送离线消息
            string msg = string.Format("PRESENCEMSG@{0}", new PresenceMsg(u.UserId, Helpers.DataHelpers.GetTimeStamp(), "127.0.0.1", Status.Offline));
            List<User> Onlines = CC.ServerHandle.GetFriendsOnline(u.UserId);
            foreach(var a in users)
            {
                foreach(var o in Onlines)
                {
                    if(a.UserId == o.UserId)
                    {
                        a.helper.SendMessage(msg);
                    }
                }
            }
        }

        public void Send2User(object sender, AddComfirmArgs args)
        {
            foreach(var u in users)
            {
                if(u.helper.tcpClient.Client.RemoteEndPoint.ToString() == args.IP)
                {
                    u.helper.SendMessage(string.Format("ADDCONFIRM@{0}", DataHelpers.XMLSer<AddConfirm>(args.comfirm)));
                }
            }
        }

        //获取本机所有适配器IP
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

        public void Update(object sender, AddCallBack callBack)
        {
            if(callBack.Sender.IsOnline==Status.Online)
            {
                var receiver = users.Where(p => p.UserId.Equals(callBack.Receiver.UserId)).FirstOrDefault();
                if(receiver.helper != null)
                {
                    receiver.helper.SendMessage(string.Format("ADDCALLBACK@{0}", DataHelpers.XMLSer<User>(callBack.Sender)));
                }
            }
            if (callBack.Receiver.IsOnline == Status.Online)
            {
                var user = users.Where(p => p.UserId.Equals(callBack.Sender.UserId)).FirstOrDefault();
                if (user.helper != null)
                {
                    user.helper.SendMessage(string.Format("ADDCALLBACK@{0}", DataHelpers.XMLSer<User>(callBack.Receiver)));
                }
            }
        }

        //为登陆用户添加Id
        public void Register(object sender, UserArgs args)
        {
            var user = users.Where(p => p.helper.tcpClient.Client.RemoteEndPoint.ToString().Equals(args.IP)).FirstOrDefault();
            if(user != null)
            {
                user.UserId = args.Id;
            }
        }

        //发送消息
        public void Talk2User(object sender, TalkContract talk)
        {
            var receiver = users.Where(p => p.UserId.Equals(talk.ReceiverId)).FirstOrDefault();
            if(receiver == null)
            {
                //存入待发列表
                CC.ServerHandle.Save2WaitSending(talk);
            }
            else
            {
                //发送给收信方
                receiver.helper.SendMessage(string.Format("TALK@{0}", DataHelpers.XMLSer<TalkContract>(talk)));
            }
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
