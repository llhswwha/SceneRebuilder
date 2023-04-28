using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Modules.Communication.WebApiObject
{
 public  class LoginInfoApi
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsEncrypted { get; set; }

        public string Session { get; set; }

        public string Authority { get; set; }

        public bool Result { get; set; }
        /// <summary>
        /// 测试bool类型传输
        /// </summary>
        public bool CsBool { get; set; }

        public string ClientIp { get; set; }

        public int ClientPort { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime LiveTime { get; set; }
    }
}
