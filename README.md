# WeGouSharp
基于搜狗的微信公众号定向爬虫，使用C#实现，故取名WeGouSharp

关于微信公共号爬虫的项目网上已经不少，然而基本大多数的都是使用Python实现
鉴于鄙人是名.NET开发人员，于是又为广大微软系同胞创建了这个轮子，使用C#实现的微信爬虫
蓝本为[Chyroc/WechatSogou](https://github.com/Chyroc/WechatSogou)，
在此还请各位大佬指教

*** 已经更新至dotnet core，可在linux或Mac下运行 ***

## 安装/引用
默认编译类型为控制台应用
也可修改为动态链接库（dll）然后在程序之间添加引用接口

## 添加依赖
- HtmlAgilityPack //用于解析HTML
- log4net //日志
- CoreCompat.System.Drawing
- OpenCvSharp3-AnyCPU //验证码显示以及自动识别
- Newtonsoft.Json //序列化

以上引用可通过NuGet添加
如(visual studio-->tools-->Nuget Package Manager-->Package Manager Console)：
```
Install-Package HtmlAgilityPack
```
~~也可以直接在项目packages文件夹获取~~


## 使用
```
dotnet run
```


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

### 公众号结构：
```C#
public struct OfficialAccount
    {

        public string AccountPageurl;
        public string WeChatId;
        public string Name;
        public string Introduction;
        public bool IsAuth; 
        public string QrCode;
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


### 公号群发消息结构(含图文推送)

```c#
   
    public struct BatchMessage
    {
        public int Meaasgeid;
        public string  SendDate;
        public string Type; //49:图文，1:文字，3:图片，34:音频，62:视频

        public string Content; 

        public string ImageUrl; 

        public string PlayLength;
        public int FileId;
        public string AudioSrc;

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
    
```

### 字段含义
字段|含义
----|----
Meaasgeid|消息号
SendDate|发出时间（unix时间戳）
Type|消息类型:49:图文， 1:文字， 3:图片， 34:音频， 62:视频
Content|文本内容（针对类型1即文字）
ImageUrl|图片（针对类型3，即图片）
PlayLength|播放长度（针对类型34，即音频，下同）
FileId|音频文件id
AudioSrc|音频源
ContentUrl|文章来源（针对类型49，即图文，下同）
Main|不明确
Title|文章标题
Digest|不明确
SourceUrl|可能是阅读原文
Cover|封面图
Author|作者
CopyrightStat|可能是否原创？
CdnVideoId|视频id（针对类型62，即视频，下同）
Thumb|视频缩略图
VideoSrc|视频链接



### 文章结构

```C#
    public struct Article
    {
        public string Url;
        public List<string>Imgs;
        public string Title;
        public string Brief;
        public string Time;
        public string ArticleListUrl;
        public OfficialAccount officialAccount;
    }
```
### 字段含义
字段|含义
----|----
Url|文章链接
Imgs|封面图（可能多个）
Title|文章标题
Brief|简介
Time|发表日期（unix时间戳）
OfficialAccount|关联的公众号（信息不全，仅供参考）



### 搜索榜结构
```C#
    public struct HotWord
    {
        public int Rank;//排行
        public string Word;
        public string JumpLink; //相关链接
        public int HotDegree; //热度
    }
```


## 其他
关于验证码，目前手动输入，后期接入自动识别或者打码平台API
欢迎各位批评，提交issue以及fork
