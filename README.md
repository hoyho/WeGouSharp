# WeGouSharp
基于搜狗的微信公众号定向爬虫，使用C#实现，故取名WeGouSharp

关于微信公共号爬虫的项目网上已经不少，然而基本大多数的都是使用Python实现
鉴于鄙人是名.NET开发人员，于是又为广大微软系同胞创建了这个轮子，使用C#实现的微信爬虫
蓝本为[Chyroc/WechatSogou](https://github.com/Chyroc/WechatSogou)
在此还请各位大佬指教

## 使用

### 初始化 API
在test中直接调用

```C#
            //创建实例
            WechatSogouApi Sogou = new WechatSogouApi();

            var result = Sogou.SearchOfficialAccount("广州大学");

            //var result = Sogou.GetOfficialAccountMessages("","bitsea",""); 
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.Write(json);
            Console.ReadKey();
```
运行截图：
![运行结果](https://github.com/hoyho/WeGouSharp/blob/master/ScreenShot/SearchOfficialAccount.png?raw=true)

## 数据结构：

### 定义：
```C#
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
```

### 字段含义
字段|含义
----|----
AccountPageurl|微信公众号页
WeChatId|公号ID（唯一)
Name|名称
Introduction|介绍
IsAuth|是否官方认证
QrCode|二维码链接
ProfilePicture|头像链接

```c#
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
```
