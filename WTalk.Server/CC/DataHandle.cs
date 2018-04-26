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
            switch (Data_Init(data)[0])
            {
                case "LOGIN":
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
