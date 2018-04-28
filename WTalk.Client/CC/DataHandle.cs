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
                case "SIGNUCALLBACK":
                    break;
                case "PRESENCEMSG":
                    break;
                case "SEARCHCALLBACK":
                    break;
                case "ADDCALLBACK":
                    break;
                case "ADDCONFIRM":
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
