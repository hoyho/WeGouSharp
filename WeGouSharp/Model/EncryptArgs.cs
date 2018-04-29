using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus.Model
{
    /// <summary>
    ///加入到链接中的一些参数
    /// </summary>
   public class EncryptArgs 
    {
        public string biz { get; set; }

        public string uin { get; set; }

        public string key { get; set; }

        public string pass_ticket { get; set; }

        public string msgid { get; set; }
    }
}
