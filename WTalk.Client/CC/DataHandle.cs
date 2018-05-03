using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTalk.Domain;
using WTalk.Helpers;

namespace WTalk.Client.CC
{
    public class DataHandle
    {
        public static event EventHandler<LoginCallBack> LoginHandler; //登陆事件
        public static event EventHandler<SignUpCallBack> SignupHandler; //注册事件
        public static event EventHandler<SearchCallBack> SearchHandler; //查找事件
        public static event EventHandler<AddConfirm> AddComfirmHandler; //好友申请事件

        public static void Handle(object sender, string data)
        {
            string[] d = Data_Init(data);
            switch(d[0])
            {
                case "LOGINCALLBACK":
                    try
                    {
                        LoginCallBack callBack = DataHelpers.DeXMLSer<LoginCallBack>(d[1]);
                        if(LoginHandler != null)
                        {
                            LoginHandler(null, callBack);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "SIGNUPCALLBACK":
                    try
                    {
                        SignUpCallBack callBack = DataHelpers.DeXMLSer<SignUpCallBack>(d[1]);
                        if(SignupHandler != null)
                        {
                            SignupHandler(null, callBack);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "PRESENCEMSG":
                    break;
                case "SEARCHCALLBACK":
                    try
                    {
                        SearchCallBack callBack = DataHelpers.DeXMLSer<SearchCallBack>(d[1]);
                        if(SearchHandler != null)
                        {
                            SearchHandler(null, callBack);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "ADDCALLBACK":
                    break;
                case "ADDCONFIRM":
                    try
                    {
                        AddConfirm confirm = DataHelpers.DeXMLSer<AddConfirm>(d[1]);
                        if(AddComfirmHandler != null)
                        {
                            AddComfirmHandler(null, confirm);
                        }
                    }
                    catch
                    {
                        break;
                    }
                    break;
                case "UPDATEFRIENDS":
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
        
    }
}
