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
using WTalk.Client.ViewModels;

namespace WTalk.Client
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window
    {        
        //一些全局变量
        private string chating = null;
        private string UserName = null;
        public ClientViewModel model = null;
        public FriendList friends = null;
        public ChatList chats = null;
        public NoticeList notices = null;
        public TCPHelper helper;
        public string LocalId;

        public UDPHelper.UDPHelper uDPHelper { get; set; }
        public ClientWindow(TCPHelper helper, List<User> users, List<TalkContract> talks, List<AddFriend> addFriends, string id, string name)
        {
            this.UserName = name;
            model = new ClientViewModel();
            friends = new FriendList();
            chats = new ChatList();
            notices = new NoticeList();
            model.AllNoticeWaitReads = 0;
            model.AllFriendWaitReads = 0;
            model.AllChatWaitReads = 0;
            #region 数据到视图模型的转换
            foreach (var u in users)
            {
                friends.Add(new Friend { UserId = u.UserId, UserName = u.UserName, IP = u.ip, status = u.IsOnline });
            }
            var senders = talks.Select(p => p.SenderId).ToList().Distinct();
            foreach(var s in senders)
            {
                MsgList m = new MsgList();
                var records = talks.Where(p => p.SenderId.Equals(s)).Select(q=>q).ToList();
                foreach (var r in records)
                {

                    m.Add(new PerMsg
                    {
                        SenderName = talks.Where(p=>p.SenderId.Equals(r.SenderId)).Select(q=>q.SenderName).FirstOrDefault(),
                        Msg = r.Content
                    });
                }
                chats.Add(new Chat { SenderId = s, SenderName = talks.Where(p=>p.SenderId.Equals(s)).Select(p=>p.SenderName).FirstOrDefault(), ReceiverId = talks.Where(x => x.SenderId.Equals(s)).Select(y => y.ReceiverId).FirstOrDefault(), Msgs = m, WaitReadNum = m.Count });
            }
            foreach(var n in addFriends)
            {
                notices.Add(new Notice { UserId = n.UserId, UserName = "DDD", status = Status.Waiting });
            }
            
            foreach(var c in chats)
            {
                model.AllChatWaitReads += c.WaitReadNum;
            }
            model.AllNoticeWaitReads = notices.Count();
            #endregion

            this.LocalId = id;

            InitializeComponent();
            this.Closing += ClientWindow_Closing;
            CC.DataHandle.UpdateFriendHandler += DataHandle_UpdateFriendHandler;
            CC.DataHandle.SearchHandler += SearchCallBack;
            CC.DataHandle.AddComfirmHandler += AddComfimCallback;
            CC.DataHandle.RemoveFriendHandler += DataHandle_RemoveFriendHandler;
            CC.DataHandle.GetMsgHandler += GetMsgHandler;
            CC.DataHandle.PresenceHandler += UpdateHandler;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.lblID.Content = LocalId;
            this.helper = helper;
            //绑定关联数据

            this.tabMain.DataContext = model;
            this.gChat.DataContext = chats;
            this.gFriend.DataContext = friends;
            this.gNotice.DataContext = notices;

            this.listChat.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(listChat_MouseLeftButtonDown), true);
            this.listChat.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(listChat_MouseRightButtonDown), true);
            this.listNotice.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(listNotice_MouseLeftButtonDown), true);

            
        }

        private void UpdateHandler(object sender, PresenceMsg e)
        {
            foreach(var f in friends)
            {
                if(f.UserId == e.UserId)
                {
                    f.status = e.status;
                    if(e.status == Status.Online)
                    {
                        f.IP = e.IP.ToString();
                    }
                    else
                    {
                        f.IP = "127.0.0.1";
                    }
                }
            }
        }


        //删除好友
        private void DataHandle_RemoveFriendHandler(object sender, RemoveContract e)
        {
            Dispatcher.Invoke(() =>
            {
                this.friends.Remove(friends.Where(p => p.UserId.Equals(e.UserId)).FirstOrDefault());
            });
        }

        private void DataHandle_UpdateFriendHandler(object sender, User e)
        {
            Friend newfriend = new Friend
            {
                UserId = e.UserId,
                UserName = e.UserName,
                IP = e.ip,
                status = e.IsOnline
            };
            Dispatcher.Invoke(() =>
            {
                friends.Add(newfriend);

            });
        }

        //好友确认回调
        private void AddComfimCallback(object sender, AddConfirm e)
        {
            this.model.AllNoticeWaitReads += 1; //通知数加一
            Notice notice = new Notice { UserId = e.UserId, UserName = e.UserName, status = Status.Waiting };
            notices.Add(notice);
        }
        //窗口关闭
        private void ClientWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(helper.tcpClient != null)
            {
                helper.br.Close();
                helper.bw.Close();
                helper.tcpClient.Close();
            }
        }

        #region 自定义标题栏
        //private void WrapPanel_MouseMove(object sender, MouseEventArgs e)
        //{
        //    try
        //    {
        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            this.DragMove();
        //        }
        //    }
        //    catch
        //    {
        //        return;
        //    }
            
        //}
        //int i = 0;
        //private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    i += 1;
        //    System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        //    timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
        //    timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };
        //    timer.IsEnabled = true;
        //    if (i % 2 == 0)
        //    {
        //        timer.IsEnabled = false;
        //        i = 0;
        //        this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        //    }
        //}
        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}
        //private void btnMin_Click(object sender, RoutedEventArgs e)
        //{
        //    this.WindowState = WindowState.Minimized;
        //}
        //private void btnMax_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.WindowState == WindowState.Maximized)
        //    {
        //        this.WindowState = WindowState.Normal;
        //    }
        //    else
        //    {
        //        this.WindowState = WindowState.Maximized;
        //    }
        //}
        #endregion

        //好友查找添加
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
            else if(friends.Where(p=>p.UserId.Equals(key)).Count() > 0)
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

        //一些通用方法
        /// <summary>
        /// 数值转换器，未读消息大于零是显示提示
        /// </summary>
        [ValueConversion(typeof(int), typeof(Visibility))]
        public class WaitShowConverter : IValueConverter
        {
            public object Convert(object value, Type TargetType, object parameter, System.Globalization.CultureInfo cultureInfo)
            {
                if (TargetType != typeof(Visibility)) { return null; }
                int num = int.Parse(value.ToString());
                return (num == 0 ? Visibility.Collapsed : Visibility.Visible);

            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
            {
                throw new NotImplementedException();
            }
        }

        //列表单击显示详情
        private void listChat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (listChat.SelectedIndex == -1)
            {
                return;
            }
            Chat boxItem = (Chat)listChat.SelectedItem;
            chating = boxItem.SenderId;
            MainChat.ItemsSource = boxItem.Msgs;
            int x = boxItem.WaitReadNum;
            model.AllChatWaitReads -= x;
            boxItem.WaitReadNum = 0;
        }

        //发送消息
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string content = txtSend.Text.Trim();
            PerMsg p = new PerMsg(UserName, content);
            Chat boxItem = (Chat)listChat.SelectedItem;
            boxItem.Msgs.Add(p);
            txtSend.Text = "";
            TalkContract talk = new TalkContract(LocalId, UserName, boxItem.SenderId, content);
            string payload = string.Format("TALK@{0}", DataHelpers.XMLSer<TalkContract>(talk));
            var receiver = friends.Where(f => f.UserId.Equals(boxItem.SenderId)).FirstOrDefault();
            if(receiver.IP != "127.0.0.1")
            {
                string[] ip = receiver.IP.Split(':');
                IPAddress address = IPAddress.Parse(ip[0]);
                IPEndPoint remote = new IPEndPoint(address, 51666);
                uDPHelper.SendMessageAsync(payload, remote);
            }
            else
            {
                helper.SendMessage(payload);
            }
        }

        //清空输入框
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PerMsg mp = new PerMsg("wang", "ceshi1");
            Chat chat = chats.Where(p => p.SenderName.Equals("wang")).FirstOrDefault();
            chat.Msgs.Add(mp);
            chat.WaitReadNum += 1;
            model.AllChatWaitReads += 1;
        }

        //列表右键菜单单击事件
        private void menu_Click(object sender, RoutedEventArgs e)
        {
            Chat chat = (Chat)this.listChat.SelectedItem;
            model.AllChatWaitReads -= chat.WaitReadNum;
            if(chating == chat.SenderId)
            {
                MainChat.ItemsSource = null;
                lblID.Content = "";
            }
            chats.Remove(chat);
            
        }
        //聊天列表右击事件
        private void listChat_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu context = new ContextMenu();
            MenuItem removethis = new MenuItem();
            removethis.Header = "删除此聊天";
            removethis.Click += Removethis_Click;
            context.Items.Add(removethis);
            if (listChat.SelectedIndex == -1)
            {
                return;
            }
            Chat boxItem = (Chat)listChat.SelectedItem;
            chating = boxItem.SenderId;
            MainChat.ItemsSource = boxItem.Msgs;
            int x = boxItem.WaitReadNum;
            model.AllChatWaitReads -= x;
            boxItem.WaitReadNum = 0;
        }
        //移除聊天项，删除消息记录
        private void Removethis_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void listNotice_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(listNotice.SelectedIndex == -1)
            {
                return;
            }
            Notice notice = (Notice)listNotice.SelectedItem;
            //NoticeDetail.DataContext = notice;
            if(notice.status == Status.Waiting)
            {
                MessageBoxResult mb = MessageBox.Show(string.Format("用户:{0}\nID:{1}\n", notice.UserName, notice.UserId), "好友申请", MessageBoxButton.OKCancel);
                AddConfirmCallBack accb = new AddConfirmCallBack();
                accb.SenderId = notice.UserId;
                accb.ReceiveId = LocalId;
                if (mb == MessageBoxResult.OK)
                {
                    accb.status = Status.Agree;
                    notice.status = Status.Agree;
                    model.AllNoticeWaitReads -= 1;
                }
                else
                {
                    accb.status = Status.DisAgree;
                    notice.status = Status.DisAgree;
                    model.AllNoticeWaitReads -= 1;
                }
                helper.SendMessage(string.Format("ADDCONFIRMCALLBACK@{0}", DataHelpers.XMLSer<AddConfirmCallBack>(accb)));
            }
            else
            {
                MessageBox.Show("已经操作过了");
            }
        }

        //同意按钮
        public void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            Notice notice = (Notice)listNotice.SelectedItem;
            AddConfirmCallBack addConfirmCallBack = new AddConfirmCallBack(notice.UserId, LocalId, Status.Agree);
            string request = string.Format("ADDCONFIRMCALLBACK@{0}", DataHelpers.XMLSer<AddConfirmCallBack>(addConfirmCallBack));
            helper.bw.Write(request);
            helper.bw.Flush();
        }
        //拒绝按钮
        public void btnDisAgree_Click(object sender, RoutedEventArgs e)
        {
            Notice notice = (Notice)listNotice.SelectedItem;
            AddConfirmCallBack addConfirmCallBack = new AddConfirmCallBack(notice.UserId, LocalId, Status.DisAgree);
            string request = string.Format("ADDCONFIRMCALLBACK@{0}", DataHelpers.XMLSer<AddConfirmCallBack>(addConfirmCallBack));
            helper.bw.Write(request);
            helper.bw.Flush();
        }
        //删除好友
        private void mDelFriend_Click(object sender, RoutedEventArgs e)
        {
            Friend friend = (Friend)this.listFriend.SelectedItem;
            friends.Remove(friend);
            //向服务器发送删除好友请求
            RemoveContract removeContract = new RemoveContract(LocalId, friend.UserId);
            helper.SendMessage(string.Format("REMOVE@{0}", DataHelpers.XMLSer<RemoveContract>(removeContract)));
        }
        //接收消息
        private void GetMsgHandler(object sender, TalkContract talk)
        {
            PerMsg msg = new PerMsg(talk.SenderName, talk.Content);
            Chat chat = chats.Where(p => p.SenderId.Equals(talk.SenderId)).FirstOrDefault();
            if(chat == null)
            {
                chat = new Chat
                {
                    SenderId = talk.SenderId,
                    SenderName = talk.SenderName,
                    ReceiverId = talk.ReceiverId,
                    Msgs = new MsgList()
                };
                chat.Msgs.Add(msg);
                Dispatcher.Invoke(() =>
                {
                    chats.Add(chat);
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    chat.Msgs.Add(msg);
                });
            }
        }

        //双击好友列表打开聊天界面
        private void listFriend_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Friend friend = (Friend)this.listFriend.SelectedItem;
            chating = friend.UserId;
            Chat c = chats.Where(p => p.SenderId.Equals(friend.UserId)).FirstOrDefault();
            if(c == null)
            {
                Chat newChat = new Chat
                {
                    SenderId = friend.UserId,
                    SenderName = friend.UserName,
                    ReceiverId = LocalId,
                    Msgs = new MsgList(),
                    WaitReadNum = 0
                };
                Dispatcher.Invoke(() =>
                {
                    chats.Add(newChat);
                    listChat.SelectedItem = newChat;
                    MainChat.ItemsSource = newChat.Msgs;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    listChat.SelectedItem = c;
                    MainChat.ItemsSource = c.Msgs;
                    int x = c.WaitReadNum;
                    model.AllChatWaitReads -= x;
                    c.WaitReadNum = 0;
                });
            }
            tabMain.SelectedIndex = 0;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            //初始化udp模块
            uDPHelper = new UDPHelper.UDPHelper();
            uDPHelper.DataHandle += CC.DataHandle.Handle;
            //Task.Run(() => udpHelper.ReceiveUdpDataAsync());
            Task.Run(() => uDPHelper.ReceiveDataAsync());
        }
    }
}
