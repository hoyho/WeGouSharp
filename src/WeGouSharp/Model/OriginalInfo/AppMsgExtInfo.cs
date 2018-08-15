using System.Collections.Generic;

namespace WeGouSharp.Model.OriginalInfo
{
    /// <summary>
    /// 群发消息内容块(封面消息+子消息)
    /// </summary>
    public class AppMsgExtInfo
    {
        public int audio_fileid { get; set; }
        public string author { get; set; }
        public string content { get; set; }
        public string content_url { get; set; }
        public int copyright_stat { get; set; }
        public string cover { get; set; }
        public int del_flag { get; set; }
        public string digest { get; set; }
        public int duration { get; set; }
        public int fileid { get; set; }
        public int is_multi { get; set; }
        public int item_show_type { get; set; }
        public int malicious_content_type { get; set; }
        public int malicious_title_reason_id { get; set; }
        public List<MultiAppMsgItemList> multi_app_msg_item_list { get; set; }
        public string play_url { get; set; }
        public string source_url { get; set; }
        public int subtype { get; set; }
        public string title { get; set; }
    }
}