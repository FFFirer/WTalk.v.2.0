using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string UserPwd { get; set; }
        public User() { }
        public User(string id, string name, string pwd)
        {
            this.UserId = id;
            this.UserName = name;
            this.UserPwd = pwd;
        }
    }
    //客户端向服务端发送的消息协议
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

    public class LogoutContract
    {
        public string UserId { get; set; }
        public LogoutContract() { }
        public LogoutContract(string id)
        {
            this.UserId = id;
        }
    }

    public class SearchContract
    {
        public string UserId { get; set; }
        public SearchContract() { }
        public SearchContract(string id)
        {
            this.UserId = id;
        }
    }

    public class AddContract
    {
        public string UserId { get; set; }
        public AddContract() { }
        public AddContract(string id)
        {
            this.UserId = id;
        }
    }

    public class RemoveContract
    {
        public string UserId { get; set; }
        public RemoveContract() { }
        public RemoveContract(string id)
        {
            this.UserId = id;
        }
    }

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

    public class AddConfirmCallBack
    {
        public string UserId { get; set; }
        public Status status { get; set; }
        public AddConfirmCallBack() { }
        public AddConfirmCallBack(string id, Status status)
        {
            this.UserId = id;
            this.status = status;
        }
    }
    public enum Status
    {
        No = 0,
        Yse = 1
    }

    //服务端向客户端发送的消息协议
    public class LoginCallBackContract
    {

    }

    public class PresenceMsg
    {

    }

    public class SearchCallBack
    {

    }

    public class AddCallBack
    {

    }

    public class AddConfirm
    {

    }

    public class UpdateFriends
    {

    }
}
