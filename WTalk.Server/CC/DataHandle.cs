using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using WTalk.Domain;

namespace WTalk.Server.CC
{
    
    public class DataHandle
    {
        public static void Handle(object sender, string data)
        {
            ServerHandle server = new ServerHandle();
            IServerHandle ish = (IServerHandle)server;
            string[] d = Data_Init(data);
            switch (d[0])
            {
                case "LOGIN":
                    LoginContract contract = null;
                    try
                    {
                        contract = WTalk.Helpers.DataHelpers.DeXMLSer<LoginContract>(d[1]);
                    }
                    catch { }
                    LoginCallBack callBack = ish.Login(contract);
                    BinaryWriter bw = (BinaryWriter)sender;
                    string msg = string.Format("LOGINCALLBACK@{0}", WTalk.Helpers.DataHelpers.XMLSer<LoginCallBack>(callBack));
                    bw.Write(msg);
                    bw.Flush();
                    break;
                case "SIGNUP":
                    break;
                case "LOGOUT":
                    break;
                case "SEARCH":
                    break;
                case "ADD":
                    break;
                case "REMOVE":
                    break;
                case "TALK":
                    break;
                case "ADDCONFIRMCALLBACK":
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
