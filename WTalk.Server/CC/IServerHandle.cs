using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;
using System.Net.Sockets;

namespace WTalk.Server.CC
{
    public interface IServerHandle
    {
        LoginCallBack Login(TcpClient client, LoginContract contract);
        SignUpCallBack Signup(SignupContract contract);
        void Logout(TcpClient client, LogoutContract contract);
        SearchCallBack Search(SearchContract contract);
        void Presence(PresenceMsg presence);
        AddComfirmArgs Add(AddContract contract);   //好友申请，向用户发送申请者ID，Name
        AddCallBack AddComfirm(AddConfirmCallBack callBack);    //好友申请确认，向申请者发送申请结果,并更新双方列表
        void RemoveFriends(RemoveContract remove);      //删除好友
    }
}
