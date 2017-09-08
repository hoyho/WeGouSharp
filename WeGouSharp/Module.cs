using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeGouSharp
{
    class Module
    {

    }

    //public struct RequestSetting
    //{
    //    public string url;
    //    public string host;
    //    public string referer;
    //    public bool proxy;
    //}

    public struct EncryptArgs //加入到链接中的一些参数
    {
        public string biz;
        public string uin;
        public string key;
        public string pass_ticket;
        public string msgid;
    }



    public struct OfficialAccount
    {

        public string AccountPageurl;
        public string WeChatId;
        public string Name;
        public string Introduction;
        public bool IsAuth; //是否官方认证账号
        public string QrCode;
       // public string Profile;
        public string ProfilePicture;//
        public string RecentArticleUrl;
    }

    //公号群发消息
    public struct BatchMessage
    {
        public int Meaasgeid;
        public string  SendDate; //时间戳
        public string Type; //49:图文，1:文字，3:图片，34:音频，62:视频

        public string Content; // for type 1

        public string ImageUrl; //for type 49

        public string PlayLength;// for type 音频
        public int FileId;// for type 音频 or 图文
        public string AudioSrc; // for type 音频

        //for type 图文
        public string ContentUrl;
        public int Main;
        public string Title;
        public string Digest;
        public string SourceUrl;
        public string Cover;
        public string Author;
        public string CopyrightStat;

        //for type 视频
        public string CdnVideoId;
        public string Thumb;
        public string VideoSrc;

        //others
    }


    public struct Article
    {
        public string Url;
        public List<string>Imgs;
        public string Title;
        public string Brief;//文章简介
        public string Time;
       // public string officialAccount;
        public string ArticleListUrl;//???
        public OfficialAccount officialAccount;
    }

    //首页搜索热词
    public struct HotWord
    {
        public int Rank;//排行
        public string Word;
        public string JumpLink;
        public int HotDegree; //热度
    }


}
