using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;

namespace WTalk.Client.CC
{
    interface IClientHandle
    {
        void LoginCallBack(LoginCallBack callBack);
        void SignupCallBack(SignUpCallBack callBack);
        void PresenceMsg(PresenceMsg msg);
        void SearchCallBack(SearchCallBack callBack);
        void AddCallBack(AddCallBack callBack);
        void AddConfirm(AddConfirm confirm);
        void UpdateFriends(UpdateFriends friends);
    }
}
