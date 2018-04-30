using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace WTalk.Domain
{
    public class Contracts
    {
        public string Operation { get; set; }
    }
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Status IsOnline { get; set; }
        public string ip { get; set; }
        public User() { }
        public User(string id, string name, Status isonline, string ip)
        {
            this.UserId = id;
            this.UserName = name;
            this.IsOnline = isonline;
            this.ip = ip;
        }
    }
    public class AddFriend
    {
        public string UserId { get; set; }
        public AddFriend() { } 
        public AddFriend(string id)
        {
            this.UserId = id;
        }
    }
    #region 客户端向服务端发送的消息协议
    //登陆
    public class LoginContract
    {
        public string UserId { get; set; }
        public string UserPwd { get; set; }

        public LoginContract() { }
        public LoginContract(string id, string pwd)
        {
            this.UserId = id;
            this.UserPwd = pwd;
        }
    }
    //注册
    public class SignupContract
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public SignupContract() { }
        public SignupContract(string name, string pwd)
        {
            this.UserName = name;
            this.UserPwd = pwd;
        }
    }
    //登出
    public class LogoutContract
    {
        public string UserId { get; set; }
        public LogoutContract() { }
        public LogoutContract(string id)
        {
            this.UserId = id;
        }
    }
    //寻找好友
    public class SearchContract
    {
        public string UserId { get; set; }
        public SearchContract() { }
        public SearchContract(string id)
        {
            this.UserId = id;
        }
    }
    //添加好友
    public class AddContract
    {
        public string SenderId { get; set; }
        public string ReveiveId { get; set; }
        public AddContract() { }
        public AddContract(string sender, string receiver)
        {
            this.SenderId = sender;
            this.ReveiveId = receiver;
        }
    }
    //删除好友
    public class RemoveContract
    {
        public string UserId { get; set; }
        public RemoveContract() { }
        public RemoveContract(string id)
        {
            this.UserId = id;
        }
    }
    //聊天消息
    public class TalkContract
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public TalkContract() { }
        public TalkContract(string sender, string receiver, string content)
        {
            this.SenderId = sender;
            this.ReceiverId = receiver;
            this.Content = content;
        }
    }
    //添加好友确认回调
    public class AddConfirmCallBack
    {
        public string SenderId { get; set; }
        public string ReceiveId { get; set; }
        public Status status { get; set; }
        public AddConfirmCallBack() { }
        public AddConfirmCallBack(string id, string receiveid, Status status)
        {
            this.SenderId = id;
            this.ReceiveId = receiveid;
            this.status = status;
        }
    }
    public enum Status
    {
        No = 0,
        Yes = 1,
        Online = 2,
        Offline = 3,
        Add = 4,
        Remove = 5,
        Agree = 6,
        DisAgree = 7,
        Waiting = 8
    }
    #endregion

    #region 服务端向客户端发送的消息协议
    //登陆回调
    public class LoginCallBack
    {
        public Status LoginStatus { get; set; } //Yes or No
        public List<User> UsersInfo { get; set; }
        public List<TalkContract> Talks { get; set; }
        public List<AddFriend> AddFriends { get; set; }
        public string msg { get; set; }

        public LoginCallBack() { }
        public LoginCallBack(Status status, List<User> usersInfo, List<TalkContract> talks, List<AddFriend> addFriends, string msg)
        {
            this.LoginStatus = status;
            this.UsersInfo = usersInfo;
            this.Talks = talks;
            this.AddFriends = addFriends;
            this.msg = msg;
        }
    }
    //注册回调
    public class SignUpCallBack
    {
        public Status status { get; set; }
        public string UserId { get; set; }
        public string MoreMsg { get; set; }
        public SignUpCallBack() { }
        public SignUpCallBack(Status status, string id, string msg)
        {
            this.status = status;
            this.UserId = id;
            this.MoreMsg = msg;
        }
    }
    //出席消息
    public class PresenceMsg
    {
        public string UserId { get; set; }
        public string Time { get; set; }
        public IPAddress IP { get; set; }
        public Status status { get; set; }

        public PresenceMsg() { }
        public PresenceMsg(string userId, string time, IPAddress ip, Status status)
        {
            this.UserId = userId;
            this.Time = time;
            this.IP = ip;
            this.status = status;
        }
    }
    //搜索回调
    public class SearchCallBack
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public SearchCallBack() { }
        public SearchCallBack(string id, string name)
        {
            this.UserId = id;
            this.UserName = name;
        }
    }
    //添加好友回调
    public class AddCallBack
    {
        public string UserId { get; set; }
        public Status status { get; set; }
        public AddCallBack() { }
        public AddCallBack(string id, Status status)
        {
            this.UserId = id;
            this.status = status;
        }
    }
    //好友申请确认
    public class AddConfirm
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public AddConfirm() { }
        public AddConfirm(string id, string name)
        {
            this.UserId = id;
            this.UserName = name;
        }
    }
    //更新好友列表
    public class UpdateFriends
    {
        public Status status { get; set; }
        public User user { get; set; }
    }
    #endregion

    public class AddComfirmArgs
    {
        public AddConfirm comfirm { get; set; }
        public string IP { get; set; }
        public AddComfirmArgs() { }
        public AddComfirmArgs(AddConfirm comfirm, string ip)
        {
            this.comfirm = comfirm;
            this.IP = ip;
        }
    }
}
