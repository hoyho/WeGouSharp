namespace WeGouSharp.Model.OriginalInfo
{
    //聚合消息原始模型
    public class Msg
    {
        public AppMsgExtInfo app_msg_ext_info { get; set; }
        public CommMsgInfo comm_msg_info { get; set; }
        public VoiceMsgInfo voice_msg_ext_info { get; set; }
        public VideoMsgExtInfo video_msg_ext_info { get; set; }
    }
}