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
using System.Net.Sockets;
using System.Net;
using System.IO;
using WTalk.Helpers;
using WTalk.Domain;

namespace WTalk.Client
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window
    {
        public TCPHelper helper;
        public List<User> Users;
        public List<TalkContract> Talks;
        public List<AddFriend> AddFriends;
        public string LocalId;
        public ClientWindow(TCPHelper helper, List<User> Users, List<TalkContract> Talks, List<AddFriend> AddFriends, string id)
        {
            this.helper = helper;
            this.Users = Users;
            this.Talks = Talks;
            this.AddFriends = AddFriends;
            this.LocalId = id;
            this.Closing += ClientWindow_Closing;
            CC.DataHandle.SearchHandler += SearchCallBack;
            CC.DataHandle.AddComfirmHandler += AddComfimCallback;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string u = "好友列表:\n";
            string t = "未读消息:\n";
            string a = "好友申请:\n";
            foreach(var U in Users)
            {
                u += string.Format("Id:{0}\nName:{1}\nStatus:{2}\nIP:{3}\n\n", U.UserId, U.UserName, U.IsOnline.ToString(), U.ip);
            }
            MessageBox.Show(u);
            foreach(var T in Talks)
            {
                t += string.Format("{0}:{1}\n", T.SenderId, T.Content);
            }
            MessageBox.Show(t);
            foreach(AddFriend As in AddFriends)
            {
                a += string.Format("{0}\n", As.UserId);
            }
            MessageBox.Show(a);
            if(helper.tcpClient!=null)
            {
                MessageBox.Show("在线");
            }
            this.lblID.Content = LocalId;
        }

        private void AddComfimCallback(object sender, AddConfirm e)
        {
            MessageBoxResult mb = MessageBox.Show(string.Format("用户:{0}\nID:{1}\n", e.UserName, e.UserId), "好友申请", MessageBoxButton.OKCancel);
            AddConfirmCallBack accb = new AddConfirmCallBack();
            accb.SenderId = e.UserId;
            accb.ReceiveId = LocalId;
            if (mb == MessageBoxResult.OK)
            {
                accb.status = Status.Agree;
            }
            else
            {
                accb.status = Status.DisAgree;
            }
            helper.SendMessage(string.Format("ADDCONFIRMCALLBACK@{0}", DataHelpers.XMLSer<AddConfirmCallBack>(accb)));
        }

        private void ClientWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(helper.tcpClient != null)
            {
                //LogoutContract logout = new LogoutContract("12");
                //helper.bw.Write("LOGOUT@"+DataHelpers.XMLSer<LogoutContract>(logout));
                //helper.bw.Flush();
                helper.br.Close();
                helper.bw.Close();
                helper.tcpClient.Close();
            }
        }

        private void WrapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch
            {
                return;
            }
            
        }

        int i = 0;
        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            i += 1;
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };
            timer.IsEnabled = true;
            if (i % 2 == 0)
            {
                timer.IsEnabled = false;
                i = 0;
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string key = txtSearchkey.Text.Trim();
            try
            {
                int t = Convert.ToInt32(key);
            }
            catch
            {
                MessageBox.Show("请输入正确的用户ID！");
            }
            if(key == LocalId)
            {
                MessageBox.Show("不能添加自己");
            }
            else if(Users.Where(p=>p.UserId.Equals(key)).Count() > 0)
            {
                MessageBox.Show("你已添加这名好友");
            }
            else
            {
                SearchContract search = new SearchContract(key);
                string request = string.Format("SEARCH@{0}", DataHelpers.XMLSer<SearchContract>(search));
                helper.bw.Write(request);
                helper.bw.Flush();
                txtSearchkey.Clear();
            }
        }

        public void SearchCallBack(object sender, SearchCallBack callback)
        {
            MessageBoxResult mbr = MessageBox.Show(string.Format("是否要添加：\n用户：{0}\nID:{1}\n为好友", callback.UserName, callback.UserId), "是否添加为好友？", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                AddContract add = new AddContract(LocalId, callback.UserId);
                helper.bw.Write(string.Format("ADD@{0}", DataHelpers.XMLSer<AddContract>(add)));
                helper.bw.Flush();
            }
        }
    }
}
