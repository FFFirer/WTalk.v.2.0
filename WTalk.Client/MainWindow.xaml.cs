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

namespace WTalk.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TCPHelper helper = new TCPHelper(IPAddress.Parse("192.168.1.105"), 51888);
        private bool isOnline = false;
        public MainWindow()
        {
            InitializeComponent();
            Connect();
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
                DataHelpers.LoginHandler += LoginSuccess;
                LoginContract login = new LoginContract(ID, Pwd);
                helper.SendMessage(string.Format("LOGIN@{0}", DataHelpers.XMLSer<LoginContract>(login)));
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            if (Pwd.Password == Pwd2.Password && Pwd.Password != "" & txtName.Text != null)
            {
                helper.SendMessage(string.Format("SIGNUP@user:{0},pwd:{1}", txtName.Text.Trim(), Pwd.Password.Trim()));
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

        public void LoginSuccess(object sender, string data)
        {
            MessageBoxResult result = MessageBox.Show(data);
            if(result == MessageBoxResult.OK)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ClientWindow cw = new ClientWindow(helper);
                    cw.Show();
                    this.Close();
                });
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
                        helper.DataHandler += DataHelpers.Handle4Client;
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
    }
}
