namespace WeGouSharp.Model
{
    /// <summary>
    /// 云打码
    /// </summary>
    public class YunDaMa
    {
        //成功的时候是返回0的
        public int ret { get; set; } = -1;

        //Code ID
        public string cid { get; set; }

        //content
        public string text { get; set; }
    }
}