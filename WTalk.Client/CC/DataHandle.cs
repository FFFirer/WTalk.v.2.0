using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTalk.Client.CC
{
    public class DataHandle
    {
        public static void Handle(object sender, string data)
        {
            
        }

        public static string[] Data_Init(string data)
        {
            string[] res = data.Split('@');
            return res;
        }
    }
}
