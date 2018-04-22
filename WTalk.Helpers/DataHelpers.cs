using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTalk.Helpers
{
    public abstract class DataHelpers
    {
        public static void Handle(string data)
        {
            string[] split = data.Split(':');
            switch (split[0])
            {
                default:
                    break;
            }
        }
    }
}
