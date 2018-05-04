using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using WTalk.Domain;
using WTalk.Helpers;

namespace WTalk.Server.CC
{
    
    public class DataHandle
    {
        public static event EventHandler<string> ShowOnServerWindow;
        public static event EventHandler<AddComfirmArgs> AddHandler;    //发送好友申请事件;
        public static event EventHandler<AddCallBack> UpdateFriendListHandler;  //更新好友列表事件
        public static event EventHandler<UserArgs> LoginHandler;  //登陆事件
        public static event EventHandler<RemoveContract> RemoveFriendHandler;   //删除好友事件

        public DataHandle()
        {
        }
        public static void Handle(object sender, string data)
        {
            ServerHandle server = new ServerHandle();
            IServerHandle ish = (IServerHandle)server;
            TCPHelper helper = (TCPHelper)sender;
            
            string[] d = Data_Init(data);
            switch (d[0])
            {
                case "LOGIN":
                    LoginContract login = null;
                    try
                    {
                        login = WTalk.Helpers.DataHelpers.DeXMLSer<LoginContract>(d[1]);
                    }
                    catch { }
                    LoginCallBack callBack = ish.Login(helper.tcpClient, login);
                    if(ShowOnServerWindow != null)
                    {
                        if (callBack.LoginStatus == Status.Yes)
                        {
                            UserArgs args = new UserArgs
                            {
                                IP = helper.tcpClient.Client.RemoteEndPoint.ToString(),
                                Id = login.UserId
                            };
                            string log = string.Format("{0}-->Login Success", helper.tcpClient.Client.RemoteEndPoint.ToString());
                            ShowOnServerWindow(null, log);
                            if(LoginHandler != null)
                            {
                                LoginHandler(null, args);
                            }
                        }
                        else
                        {
                            string log = string.Format("{0}-->Login Fail", helper.tcpClient.Client.RemoteEndPoint.ToString());
                            ShowOnServerWindow(null, log);
                        }
                    }
                    string msg = string.Format("LOGINCALLBACK@{0}", WTalk.Helpers.DataHelpers.XMLSer<LoginCallBack>(callBack));
                    helper.SendMessage(msg);
                    break;
                case "SIGNUP":
                    SignupContract signup = null;
                    try
                    {
                        signup = DataHelpers.DeXMLSer<SignupContract>(d[1]);
                    }
                    catch{ }
                    SignUpCallBack signUpCallBack = ish.Signup(signup);
                    if (ShowOnServerWindow != null)
                    {
                        string log = string.Format("{0}-->Signup:{1}", helper.tcpClient.Client.RemoteEndPoint.ToString(), signUpCallBack.MoreMsg);
                        //if (callBack.LoginStatus == Status.Yes)
                        //{
                        //    string log = string.Format("{0}-->Signup", helper.tcpClient.Client.RemoteEndPoint.ToString());
                        //    ShowOnServerWindow(null, log);
                        //}
                        //else
                        //{
                        //    string log = string.Format("{0}-->Login Fail", helper.tcpClient.Client.RemoteEndPoint.ToString());
                        //    ShowOnServerWindow(null, log);
                        //}
                        ShowOnServerWindow(null, log);
                    }
                    helper.SendMessage(string.Format("SIGNUPCALLBACK@{0}", DataHelpers.XMLSer<SignUpCallBack>(signUpCallBack)));
                    break;
                case "LOGOUT":
                    //string logoutlog = string.Format("{0}-->登出", helper.tcpClient.Client.RemoteEndPoint.ToString());
                    //if(ShowOnServerWindow != null)
                    //{
                    //    ShowOnServerWindow(null, logoutlog);
                    //}
                    break;
                case "SEARCH":
                    SearchContract search = null;
                    try
                    {
                        search = DataHelpers.DeXMLSer<SearchContract>(d[1]);
                        SearchCallBack back = ish.Search(search);
                        helper.SendMessage(string.Format("SEARCHCALLBACK@{0}", DataHelpers.XMLSer<SearchCallBack>(back)));
                    }
                    catch
                    {

                    }
                    break;
                case "ADD":
                    AddContract add = null;
                    try
                    {
                        add = DataHelpers.DeXMLSer<AddContract>(d[1]);
                        AddComfirmArgs args = ish.Add(add);
                        if (args.IP != "Offline")
                        {
                            AddHandler(null, args);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "REMOVE":
                    RemoveContract remove = null;
                    try
                    {
                        remove = DataHelpers.DeXMLSer<RemoveContract>(d[1]);
                        ish.RemoveFriends(remove);
                        if(RemoveFriendHandler != null)
                        {
                            RemoveFriendHandler(null, remove);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    break;
                case "TALK":
                    break;
                case "ADDCONFIRMCALLBACK":
                    AddConfirmCallBack accb = null;
                    AddCallBack addCallBack = null;
                    try
                    {
                        accb = DataHelpers.DeXMLSer<AddConfirmCallBack>(d[1]);
                        addCallBack = ish.AddComfirm(accb);
                        if (UpdateFriendListHandler != null)
                        {
                            UpdateFriendListHandler(null, addCallBack);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                default:
                    break;
            }
        }

        public static string[] Data_Init(string data)
        {
            string[] res = data.Split('@');
            return res;
        }

        public static void ClearPresence()
        {
            ServerHandle server = new ServerHandle();
            server.RefreshAll();
        }
    }

    //public class UserArgs
    //{
    //    public string IP { get; set; }
    //    public string Id { get; set; }
    //}
}
