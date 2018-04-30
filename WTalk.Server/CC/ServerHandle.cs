﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;
using WTalk.Helpers;
using WTalk.Server.Data;
using System.Net;
using System.Net.Sockets;

namespace WTalk.Server.CC
{
    public class ServerHandle : IServerHandle
    {
        public ServerHandle()
        {
            TCPHelper.ExitHandler += RemoveUser;
            //model = new DataModel();
        }
        //用户登陆
        public LoginCallBack Login(TcpClient client, LoginContract contract)
        {
            using(var model = new Data.DataModel())
            {
                LoginCallBack callback = new LoginCallBack();
                var user = model.users.Where(p => p.UserId.Equals(contract.UserId)).FirstOrDefault();
                if (user.Password == contract.UserPwd)
                {
                    //将该用户写入出席服务
                    model.presenceusers.Add(new presenceuser { UserId = contract.UserId, IPAddress = client.Client.RemoteEndPoint.ToString(), PresenceTime = DataHelpers.GetTimeStamp(), Status = Status.Online.ToString() });

                    //获取好友列表
                    var friends = model.friends.Where(p => p.UserId.Equals(user.UserId)).Select((p) => new User
                    {
                        UserId = p.FriendId,
                        UserName = model.users.Where(q => q.UserId.Equals(p.FriendId)).Select(q => q.UserName).FirstOrDefault(),
                        IsOnline = Status.Offline,
                        ip = "127.0.0.1"
                    }).ToList();
                    //获取在线列表
                    var friendsInfo = model.friends.Join(model.presenceusers, a => a.FriendId, b => b.UserId, (a, b) => new User
                    {
                        UserId = b.UserId,
                        UserName = model.users.Where(p => p.UserId.Equals(a.UserId)).Select(p => p.UserName).FirstOrDefault(),
                        IsOnline = Status.Online,
                        ip = b.IPAddress
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
                    var talks = model.waitforsendings.Where((p) => p.ReceiverId.Equals(contract.UserId)).Select(p => new TalkContract {
                        Content=p.Msg,
                        SenderId=p.SenderId,
                        ReceiverId=p.ReceiverId
                    }).ToList();
                    //获取待同意好友申请
                    var adds = model.addfriends.Where((p) => p.ReceiverId.Equals(contract.UserId) && p.Status.Equals(Status.Waiting.ToString())).Select((p) => new AddFriend {
                        UserId = p.SenderId,

                    }).ToList();
                    callback = new LoginCallBack(Status.Yes, friends, talks, adds, null);
                    model.SaveChanges();    //刷新数据库
                    return callback;
                }
                else 
                {
                    List<User> friends = null;
                    List<TalkContract> talks = null;
                    List<AddFriend> adds = null;
                    callback = new LoginCallBack(Status.No, friends, talks, adds, "登陆失败");
                    return callback;
                }
            }
        }

        //用户注册
        public SignUpCallBack Signup(SignupContract contract)
        {
            SignUpCallBack call = new SignUpCallBack();
            string Id = string.Empty;
            using (var model = new Data.DataModel())
            {
                //获取全局唯一的用户Id
                while(true)
                {
                    Id = GetARandomId();
                    var cf = model.users.Where(p => p.UserId.Equals(Id)).Count();
                    if (cf <= 0)
                    {
                        break;
                    }
                }
                //将用户ID插入数据库
                try
                {
                    model.users.Add(new user { UserId = Id, UserName = contract.UserName, Password = contract.UserPwd });
                    model.SaveChanges();
                    call.status = Status.Yes;
                    call.UserId = Id;
                    call.MoreMsg = "注册成功！";
                }
                catch(Exception e)
                {
                    call.status = Status.No;
                    call.UserId = null;
                    call.MoreMsg = "注册失败:" + e.Message;
                }
                return call;
            }
        }

        public void Logout(TcpClient client, LogoutContract contract)
        {
            //RemoveUser(null, client.Client.RemoteEndPoint.ToString());
            return;
        }

        public SearchCallBack Search(SearchContract contract)
        {
            using(var model = new Data.DataModel())
            {
                var user = model.users.Where(p => p.UserId.Equals(contract.UserId)).FirstOrDefault();
                SearchCallBack callBack = new SearchCallBack(user.UserId, user.UserName);
                return callBack;
            }
        }
        public void Presence(PresenceMsg presence)
        {
            return;
        }
        //好友申请，向用户发送申请者ID，Name
        public AddComfirmArgs Add(AddContract contract)
        {
            AddConfirm call = null;
            AddComfirmArgs args = null;
            using (var model = new Data.DataModel())
            {
                try
                {
                    model.addfriends.Add(new addfriend { TIme = DataHelpers.GetTimeStamp(), SenderId = contract.SenderId, ReceiverId = contract.ReveiveId, Status = Status.Waiting.ToString() });
                    var user = model.presenceusers.Where(p => p.UserId.Equals(contract.ReveiveId)).FirstOrDefault();
                    var sender = model.users.Where(p => p.UserId.Equals(contract.SenderId)).FirstOrDefault();
                    if (user != null)
                    {
                        call = new AddConfirm(sender.UserId, sender.UserName);
                        args = new AddComfirmArgs(call, user.IPAddress);
                    }
                    else
                    {
                        call = new AddConfirm(contract.SenderId, "未知");
                        args = new AddComfirmArgs(call, "Offline");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                
                var a = model.SaveChanges();
                return args;
            }
        }

        //好友申请确认，向申请者发送申请结果,并更新双方列表
        public AddCallBack AddComfirm(AddConfirmCallBack callBack)
        {
            AddCallBack call = null;
            using(var model = new Data.DataModel())
            {
                var s = model.addfriends.Where(p => p.SenderId.Equals(callBack.SenderId) && p.ReceiverId.Equals(callBack.ReceiveId)).Select(p=>p).FirstOrDefault();
                //将好友申请操作列表里的所有两方的操作状态全部改成与申请返回的值一样的
                s.Status = callBack.status.ToString();
                model.Entry(s).State = System.Data.Entity.EntityState.Modified;

                if (callBack.status.ToString() == Status.Agree.ToString())
                {
                    friend f1 = new friend { UserId = callBack.SenderId, FriendId = callBack.ReceiveId };
                    friend f2 = new friend { UserId = callBack.ReceiveId, FriendId = callBack.SenderId };
                    model.friends.Add(f1);
                    model.friends.Add(f2);
                }
                var a = model.SaveChanges();
                return call;
            }
        }

        //当用户掉线或推出时，将用户从出席服务中移除
        public void RemoveUser(object sender, string IP)
        {
            using(var model = new Data.DataModel())
            {
                model.presenceusers.Remove(model.presenceusers.Where(p => p.IPAddress.Equals(IP)).FirstOrDefault());
                model.SaveChanges();
            }
        }

        #region 一些要用到的重复的方法
        public string GetARandomId()
        {
            Random random = new Random();
            int res = random.Next(999999, 9999999);
            return res.ToString();
        }

        //清楚在线列表
        public void RefreshAll()
        {
            using(var model = new DataModel())
            {
                foreach(var user in model.presenceusers)
                {
                    model.presenceusers.Remove(user);
                }
                model.SaveChanges();
            }
        }
        #endregion
    }
}
