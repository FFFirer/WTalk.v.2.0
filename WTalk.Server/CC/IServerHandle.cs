using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;

namespace WTalk.Server.CC
{
    public interface IServerHandle
    {
        LoginCallBack Login(LoginContract contract);
        SignUpCallBack Signup(SignupContract contract);
        void Logout(LogoutContract contract);
        SearchCallBack Search(SearchContract contract);
        void Presence(PresenceMsg presence);
        AddConfirm Add(AddContract contract);   //好友申请，向用户发送申请者ID，Name
        AddCallBack AddComfirm(AddConfirmCallBack callBack);    //好友申请确认，向申请者发送申请结果,并更新双方列表
    }
}
