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
using WTalk.Domain;
using WTalk.Helpers;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace WTalk.Client
{
    /// <summary>
    /// SignupWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    //已弃用
    public partial class SignupWindow : Window
    {
        public TCPHelper tcpHelper;
        
        public SignupWindow()
        {
            InitializeComponent();
            DataHelpers.ShowHandler += this.ShowMsg;
            Task.Run(() => tcpHelper.ReceiveData());
        }
        

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            if(Pwd.Password == Pwd2.Password)
            {
                SignupContract signup = new SignupContract(txtName.Text.Trim(), Pwd.Password.Trim());
                //tcpHelper.SendMessage(string.Format("SIGNUP@user:{0},pwd:{1}", txtName.Text.Trim(), Pwd.Password.Trim()));
            }
            else
            {
                ShowMsg(null, "密码不符合");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            Pwd.Clear();
            Pwd2.Clear();
        }

        //展示消息
        public void ShowMsg(object sender, string data)
        {
            MessageBox.Show(data);
        }

        //触发注册回调
        public void SignupHandle(object sender, SignUpCallBack callBack)
        {
            string res = string.Empty;
            if(callBack.status == Status.Yes)
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
