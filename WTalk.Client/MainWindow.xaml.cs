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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WTalk.Domain;
using WTalk.Helpers;
using System.Net.Sockets;
using System.IO;
using System.Net;
using WTalk.Client.ViewModels;

namespace WTalk.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string DefaultIP = "192.168.43.171";
        public static int DefaultPort = 51888;
        private TCPHelper helper = new TCPHelper(IPAddress.Parse(DefaultIP), DefaultPort);
        private bool isOnline = false;
        public MainWindow()
        {
            InitializeComponent();
            Connect();
            CC.DataHandle.SignupHandler += SignupHandle;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            if(txtId.Text == "" || txtPwd.Password == "")
            {
                ShowMsg(null, "请输入用户ID和密码");
            }
            else
            {
                string ID = txtId.Text.Trim();
                string Pwd = txtPwd.Password.Trim();
                CC.DataHandle.LoginHandler += LoginSuccess;
                LoginContract login = new LoginContract(ID, Pwd);
                helper.SendMessage(string.Format("LOGIN@{0}", DataHelpers.XMLSer<LoginContract>(login)));
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            if (Pwd.Password == Pwd2.Password && Pwd.Password != "" & txtName.Text != null)
            {
                SignupContract signupContract = new SignupContract(txtName.Text.Trim(), Pwd.Password.Trim());
                //helper.SendMessage(string.Format("SIGNUP@user:{0},pwd:{1}", txtName.Text.Trim(), Pwd.Password.Trim()));
                helper.SendMessage(string.Format("SIGNUP@{0}", DataHelpers.XMLSer<SignupContract>(signupContract)));
            }
            else
            {
                ShowMsg(null, "密码不符合");
            }
        }

        public void ShowMsg(object sender, string data)
        {
            MessageBox.Show(data);
        }

        public void LoginSuccess(object sender, LoginCallBack data)
        {
            if(data.LoginStatus == Status.Yes)
            {
                MessageBoxResult result = MessageBox.Show("登陆成功");
                
                if (result == MessageBoxResult.OK)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ClientWindow cw = new ClientWindow(helper, data.UsersInfo, data.Talks, data.AddFriends, txtId.Text.Trim());
                        cw.Show();
                        this.Close();
                    });
                }
            }
            else
            {
                MessageBox.Show(data.msg);
            }
        }

        public void Connect()
        {
            if(isOnline == false)
            {
                try
                {
                    helper.Ready();
                    if (helper.tcpClient != null)
                    {
                        DataHelpers.ShowHandler += ShowMsg;
                        helper.DataHandler += CC.DataHandle.Handle;
                        Task.Run(() => helper.ReceiveData());
                        isOnline = true;
                    }
                    else
                    {
                        ShowMsg(null, "无法连接到服务器");
                    }
                }
                catch (Exception e)
                {
                    ShowMsg(null, e.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            Pwd.Clear();
            Pwd2.Clear();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            string Ip = txtIP.Text.Trim();
            int port = Convert.ToInt32(txtPort.Text.Trim());
            helper = new TCPHelper(IPAddress.Parse(Ip), port);
            MessageBox.Show("设置成功！");
        }
        //触发注册回调
        public void SignupHandle(object sender, SignUpCallBack callBack)
        {
            string res = string.Empty;
            if (callBack.status == Status.Yes)
            {
                res = string.Format("{0}\nID：{1}\n记住ID以登陆账户", callBack.MoreMsg, callBack.UserId);
            }
            else
            {
                res = callBack.MoreMsg;
            }
            ShowMsg(null, res);
        }
    }
}
