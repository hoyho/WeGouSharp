namespace WeGouSharpPlus.Model
{
    /// <summary>
    /// 公众号
    /// </summary>
   public class OfficialAccount
    {
        public string AccountPageurl { get; set; }

        public string WeChatId { get; set; }

        public string Name { get; set; }

        public string Introduction { get; set; }

        /// <summary>
        /// //是否官方认证账号
        /// </summary>
        public bool IsAuth { get; set; } 

        public string QrCode { get; set; }

        // public string Profile;
        public string ProfilePicture { get; set; }

        public string RecentArticleUrl { get; set; }
    }
}
