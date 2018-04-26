using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;
using WTalk.Server.Data;
using System.Net;

namespace WTalk.Server.CC
{
    public class ServerHandle : IServerHandle
    {
        public Data.DataModel model { get; set; }
        public LoginCallBack Login(LoginContract contract)
        {
            using(var db = new Data.DataModel())
            {
                var user = db.users.Where(p => p.UserId.Equals(contract.UserId)).FirstOrDefault();
                if (user.Password == contract.UserPwd)
                {
                    //获取好友列表
                    var friends = db.friends.Where(p => p.UserId.Equals(user.UserId)).Select((p) => new User
                    {
                        UserId = p.UserId,
                        UserName = db.users.Where(q => q.UserId.Equals(p.UserId)).Select(q => q.UserName).FirstOrDefault(),
                        IsOnline = Status.Offline,
                        ip = null
                    }).ToList();
                    //获取在线列表
                    var friendsInfo = db.friends.Join(db.presenceusers, a => a.FriendId, b => b.UserId, (a, b) => new User
                    {
                        UserId = b.UserId,
                        UserName = db.users.Where(p => p.UserId.Equals(a.UserId)).Select(p => p.UserName).FirstOrDefault(),
                        IsOnline = Status.Online,
                        ip = IPAddress.Parse(b.IPAdderss)
                    }).ToList();
                    //合并两个文件
                    foreach(var i in friendsInfo)
                    {
                        foreach(var i2 in friends)
                        {
                            if (i.UserId.Equals(i2.UserId))
                            {
                                i2.IsOnline = i.IsOnline;
                                i2.ip = i.ip;
                            }
                        }
                    }
                    //获取待接收消息
                    var talks = db.waitforsendings.Where((p) => p.ReceiverId.Equals(contract.UserId)).Select(p => new TalkContract {
                        Content=p.Msg,
                        SenderId=p.SenderId,
                        ReceiverId=p.ReceiverId
                    }).ToList();
                    //获取待同意好友申请
                    var adds = db.addfriends.Where((p) => p.ReceiverId.Equals(contract.UserId)).Select((p) => new AddFriend {
                        UserId = p.SenderId,

                    }).ToList();
                    LoginCallBack callBack = new LoginCallBack(Status.Yes, friends, talks, adds, null);
                }
            }
        }
        public SignUpCallBack Signup(SignupContract contract)
        {

        }
        public void Logout(LogoutContract contract)
        {

        }
        public SearchCallBack Search(SearchContract contract)
        {

        }
        public void Presence(PresenceMsg presence)
        {

        }
        //好友申请，向用户发送申请者ID，Name
        public AddConfirm Add(AddContract contract)
        {

        }
        //好友申请确认，向申请者发送申请结果,并更新双方列表
        public AddCallBack AddComfirm(AddConfirmCallBack callBack)
        {

        }
    }
}
