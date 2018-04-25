using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;

namespace WTalk.Helpers
{
    public static class DataHelpers
    {
        public static event EventHandler<string> ShowHandler;
        public static event EventHandler<string> LoginHandler;
        //基础数据处理
        public static void Handle(object sender, string data)
        {
            string[] split = data.Split('@');
            switch (split[0])
            {
                default:
                    break;
            }
        }
        //客户端用
        public static void Handle4Client(object sender, string data)
        {
            string[] split = data.Split('@');
            switch (split[0])
            {
                case "LOGINCALLBACK":
                    if (split[1] == "SUCCESS/FAILURE") LoginHandler(null, split[1]);
                    else ShowHandler(null, split[1]);
                    break;
                case "SIGNUPCALLBACK":
                    ShowHandler(null, split[1]);
                    break;
                case "LOGOUTCALLBACK":
                    break;
                case "TALKCALLBACK":
                    break;
                case "SEARCHCALLBACK":
                    break;
                case "ADDCALLBACK":
                    break;
                case "REMOVECALLBACK":
                    break;
                case "UPDATECALLBACK":
                    break;
                default:
                    ShowHandler(null, data);
                    break;
            }
        }
        //服务器端用
        public static void Handle4Server(object sender, string data)
        {
            BinaryWriter bw2c;
            try
            {
                bw2c = (BinaryWriter)sender;
            }
            catch
            {
                data = "服务器端数据处理失败";
                return;
            }
            string[] split = data.Split('@');
            switch (split[0])
            {
                case "LOGIN":
                    ShowHandler(null, split[0]);
                    bw2c.Write(string.Format("LOGINCALLBACK@SUCCESS/FAILURE"));
                    bw2c.Flush();
                    break;
                case "SIGNUP":
                    ShowHandler(null, split[0]);
                    bw2c.Write(string.Format("SIGNUPCALLBACK@SUCCESS/FAILURE"));
                    bw2c.Flush();
                    break;
                case "LOGOUT":
                    break;
                case "TALK":
                    break;
                case "SEARCH":
                    break;
                case "ADD":
                    break;
                case "REMOVE":
                    break;
                case "UPDATE":
                    break;
                default:
                    TcpClient client = (TcpClient)sender;
                    data = client.Client.RemoteEndPoint.ToString() + "-->" + data;
                    ShowHandler(null, data);
                    break;
            }
        }

        //数据序列化
        //对象序列化XML字符串
        public static string XMLSer<T>(T entity)
        {
            StringBuilder builder = new StringBuilder();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StringWriter(builder))
            {
                xs.Serialize(writer, entity);
            }

            return builder.ToString();
        }
        //XML字符串反序列化为对象
        public static T DeXMLSer<T>(string xmlString)
        {
            T cloneObject = default(T);
            StringBuilder builder = new StringBuilder();
            builder.Append(xmlString);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (TextReader reader = new StringReader(builder.ToString()))
            {
                Object obj = serializer.Deserialize(reader);
                cloneObject = (T)obj;
            }

            return cloneObject;
        }
    }
}
