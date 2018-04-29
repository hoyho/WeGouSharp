using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WeGouSharpPlus.Model;

namespace WeGouSharpPlus
{
    class Basic
    {
    }


    public class WechatSogouBasic
    {
        ILog logger = LogManager.GetLogger(typeof(Program));


        string _vcode_url = "";

        WechatCache weChatCache;
        public static List<string> _agent = new List<string>
        {
            "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; AcooBrowser; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Acoo Browser; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506)",
            "Mozilla/4.0 (compatible; MSIE 7.0; AOL 9.5; AOLBuild 4337.35; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
            "Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 2.0.50727; Media Center PC 6.0)",
            "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 1.0.3705; .NET CLR 1.1.4322)",
            "Mozilla/4.0 (compatible; MSIE 7.0b; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506.30)",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN) AppleWebKit/523.15 (KHTML, like Gecko, Safari/419.3) Arora/0.3 (Change: 287 c9dfb30)",
            "Mozilla/5.0 (X11; U; Linux; en-US) AppleWebKit/527+ (KHTML, like Gecko, Safari/419.3) Arora/0.6",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.2pre) Gecko/20070215 K-Ninja/2.1.1",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9) Gecko/20080705 Firefox/3.0 Kapiko/3.0",
            "Mozilla/5.0 (X11; Linux i686; U;) Gecko/20070322 Kazehakase/0.4.5",
            "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.8) Gecko Fedora/1.9.0.8-1.fc10 Kazehakase/0.5.6",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_3) AppleWebKit/535.20 (KHTML, like Gecko) Chrome/19.0.1036.7 Safari/535.20",
            "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; fr) Presto/2.9.168 Version/11.52",
        };


        public WechatSogouBasic()
        {
            this.weChatCache = new WechatCache(Config.CacheDir, 60 * 60);
            //tofix
            //if (weChatCache.Get<HttpWebRequest>(Config.CacheSessionName) != null)
            //{
            //    //todo

            //}
        }
        int _tryCount;






        /// <summary>
        /// 通过搜狗搜索指定关键字返回的文本
        /// </summary>
        /// <param name="name">搜索关键字</param>
        /// <param name="page">搜索的页数</param>
        /// <returns>返回的html string</returns>
        protected string _SearchAccount_Html(string name, int page = 1, int tryTime = 1)
        {
            string text = "";
            WebHeaderCollection headers = new WebHeaderCollection();
            HttpHelper netHelper = new HttpHelper();
            string requestUrl = string.Format("http://weixin.sogou.com/weixin?query={0}&_sug_type_=&_sug_=n&type=1&page={1}&ie=utf8", name, page);

            try
            {
                text = tryTime > 5 ? "" : netHelper.Get(headers, requestUrl, "utf-8", true);
            }          
            catch (WechatSogouVcodeException e)
            {
                if (e.Message == "weixin.sogou.com verification code")
                {
                    //this._jieFeng();
                    netHelper.UnLock(false);
                    //RequestSetting  requestSetting = new RequestSetting () { host = "", referer = "http://weixin.sogou.com/antispider/?from=%2f" + this._vcode_url.Replace("http://", "") };

                    headers.Add("host", "");
                    headers.Add("refer", "http://weixin.sogou.com/antispider/?from=%2f" + this._vcode_url.Replace("http://", ""));
                    text = netHelper.Get(headers, requestUrl);
                }

            }

            return text;

        }


        /// <summary>
        /// 通过搜狗搜索微信文章关键字返回纯html字符串
        /// </summary>
        /// <param name="name">搜索文章关键字</param>
        /// <param name="page">搜索的页数</param>
        /// <returns>HTML string</returns>
        protected string _SearchArticle_Html(string name, int page)
        {

            string requestUrl = "http://weixin.sogou.com/weixin?query=" + name + "&_sug_type_=&_sug_=n&type=2&page=" + page + "&ie=utf8";
            string text = "";
            HttpHelper browser = new HttpHelper();
            try
            {
                WebHeaderCollection headers = new WebHeaderCollection();
                text = browser.Get(headers, requestUrl);
            }
            catch (Exception e)
            {
                if (e.Message == "weixin.sogou.com verification code")
                {
                    browser.UnLock(false);
                    //RequestSetting  requestSetting= new RequestSetting () { host = "", referer = "http://weixin.sogou.com/antispider/?from=%2f" + this._vcode_url.Replace("http://", "") };
                    WebHeaderCollection headers = new WebHeaderCollection();
                    headers.Add("referer", "http://weixin.sogou.com/antispider/?from=%2f" + this._vcode_url.Replace("http://", ""));
                    HttpHelper netHelper = new HttpHelper();
                    text = netHelper.Get(headers, requestUrl);
                }

            }
            return text;
        }


        /// <summary>
        /// 获取最近文章页的文本
        /// </summary>
        /// <param name="url">最近文章页地址</param>
        /// <returns></returns>
        public string _GetRecentArticle_Html(string url)
        {

            //RequestSetting requestSetting = new RequestSetting() { host = "mp.weixin.qq.com" };
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("host", "mp.weixin.qq.com");
            HttpHelper netHelper = new HttpHelper();
            string text = netHelper.Get(headers, url);
            //string text = netHelper.Get(url);
            _tryCount = 1;
            //netHelper.UnLock(false );
            if (text.Contains("为了保护你的网络安全，请输入验证码") || _tryCount > 1)
            {
                //to do 解封
                //result = self._ocr_for_get_gzh_article_by_url_text(url)

                netHelper.VerifyCodeForContinute(url, false);
                // netHelper.UnblockFrequencyLimit(url,true);
                //解封后再次请求
                text = netHelper.Get(headers, url, "UTF-8", true);
                if (text.Contains("验证码有误"))
                {
                    Console.WriteLine("验证时输入错误");
                    if (_tryCount > 1)
                    {
                        throw new Exception(string.Format("验证码识别错误 url:{0}", url));
                    }
                }
                this._GetRecentArticle_Html(url);
            }
            return text;
        }



        /// <summary>
        /// 最近文章页  公众号信息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public OfficialAccount _ResolveOfficialAccount(string htmlText, string url)
        {
            OfficialAccount officialAccount = new OfficialAccount();
            officialAccount.AccountPageurl = url;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlText);
            var profileInfoArea = doc.DocumentNode.SelectSingleNode("//div[@class='profile_info_area']");
            officialAccount.ProfilePicture = profileInfoArea.SelectSingleNode("div[1]/span/img").GetAttributeValue("src", "");
            officialAccount.Name = profileInfoArea.SelectSingleNode("div[1]/div/strong/text()").InnerText.Trim();
            //name = Tools.replace_space(name);
            string wechatId = profileInfoArea.SelectSingleNode("div[1]/div/p/text()").InnerText.Trim();
            if (wechatId.Length > 0)
            {
                wechatId = wechatId.Replace("微信号: ", "");

            }
            else
            {
                wechatId = "";
            }

            officialAccount.WeChatId = wechatId;
            officialAccount.Introduction = profileInfoArea.SelectSingleNode("ul/li[1]/div/text()").InnerText;
            string authInfo = profileInfoArea.SelectSingleNode("ul/li[2]/div/text()").InnerText;

            if (!string.IsNullOrEmpty(authInfo))
            {
                officialAccount.IsAuth = true;
            }
            else
            {
                officialAccount.IsAuth = false;
            }

            string qrcode = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//*[@id='js_pc_qr_code_img']").GetAttributeValue("src", ""));
            if (qrcode.Length > 0)
            {
                qrcode = "http://mp.weixin.qq.com" + qrcode;
            }
            else
            {
                qrcode = "";
            }

            officialAccount.QrCode = qrcode;
            return officialAccount;

        }



        /// <summary>
        /// 从网页内容提取出最近文章内容（json）
        /// </summary>
        /// <param name="text"></param>原来的_get_gzh_article_by_url_dict
        /// 
        /// <returns>list of BatchMessage</returns>
        protected string _ExtracJson(string text)
        {
            string msglist = "";
            Regex reg = new Regex("var msgList =(.+?)};");
            Match match = reg.Match(text);
            msglist = match.Groups[1].Value;

            msglist = msglist + "}";
            var msgdict = WebUtility.HtmlDecode(msglist);
            //var msgdict = Tools.replace_html(msglist);
            return msgdict;
        }


        /// <summary>
        /// 解析 公众号 群发消息
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encryp"></param>
        /// <returns></returns>
        protected List<BatchMessage> _ResolveBatchMessageFromJson(string jsonText, EncryptArgs encryp)
        {
            List<BatchMessage> messages = new List<BatchMessage>();
            string biz = encryp.biz;
            string uin = encryp.uin;
            string key = encryp.key;

            JObject jo = JObject.Parse(jsonText);//把json字符串转化为json对象  
            JArray msgList = (JArray)jo.GetValue("list");
            foreach (JObject msg in msgList)
            {

                BatchMessage aMessage = new BatchMessage();
                JObject commMsgInfo = (JObject)msg.GetValue("comm_msg_info");
                aMessage.Meaasgeid = (int)commMsgInfo.GetValue("id");
                aMessage.SendDate = (string)commMsgInfo.GetValue("datetime");
                aMessage.Type = (string)commMsgInfo.GetValue("type");
                switch (aMessage.Type)
                {
                    case "1": //文字
                        aMessage.Content = (string)commMsgInfo.GetValue("");
                        break;
                    case "3": //图片
                        aMessage.ImageUrl = "https://mp.weixin.qq.com/mp/getmediadata?__biz=" + biz + "&type=img&mode=small&msgid=" + aMessage.Meaasgeid + "&uin=" + uin + "&key=" + key;
                        break;
                    case "34": //音频
                        aMessage.PlayLength = (string)msg.SelectToken("voice_msg_ext_info.play_length");
                        aMessage.FileId = (int)msg.SelectToken("voice_msg_ext_info.fileid");
                        aMessage.AudioSrc = "https://mp.weixin.qq.com/mp/getmediadata?biz=" + biz + "&type=voice&msgid=" + aMessage.Meaasgeid + "&uin=" + uin + "&key=" + key;
                        break;
                    case "49": //图文
                        JObject AppMsgExtInfo = (JObject)msg.GetValue("app_msg_ext_info");
                        string url = (string)AppMsgExtInfo.GetValue("content_url");
                        if (!String.IsNullOrEmpty(url))
                        {
                            if (!url.Contains("http://mp.weixin.qq.com")) { url = "http://mp.weixin.qq.com" + url; }

                        }
                        else
                        {
                            url = "";
                        }
                        aMessage.Main = 1;
                        aMessage.Title = (string)AppMsgExtInfo.GetValue("title");
                        aMessage.Digest = (string)AppMsgExtInfo.GetValue("digest");
                        aMessage.FileId = (int)AppMsgExtInfo.GetValue("fileid");
                        aMessage.ContentUrl = url;
                        aMessage.SourceUrl = (string)AppMsgExtInfo.GetValue("source_url");
                        aMessage.Cover = (string)AppMsgExtInfo.GetValue("cover");
                        aMessage.Author = (string)AppMsgExtInfo.GetValue("author");
                        aMessage.CopyrightStat = (string)AppMsgExtInfo.GetValue("copyright_stat");
                        messages.Add(aMessage);

                        if ((int)AppMsgExtInfo.GetValue("is_multi") == 1)
                        {
                            JArray multi_app_msg_item_list = (JArray)AppMsgExtInfo.GetValue("multi_app_msg_item_list");
                            BatchMessage moreMessage = new BatchMessage();
                            foreach (JObject subMsg in multi_app_msg_item_list)
                            {
                                url = (string)subMsg.GetValue("content_url");
                                if (!string.IsNullOrEmpty(url))
                                {
                                    if (!url.Contains("http://mp.weixin.qq.com")) { url = "http://mp.weixin.qq.com" + url; }
                                }
                                else
                                {
                                    url = "";
                                }

                                moreMessage.Title = (string)subMsg.GetValue("title");
                                moreMessage.Digest = (string)subMsg.GetValue("digest");
                                moreMessage.FileId = (int)subMsg.GetValue("fileid");
                                moreMessage.ContentUrl = url;
                                moreMessage.SourceUrl = (string)subMsg.GetValue("source_url");
                                moreMessage.Cover = (string)subMsg.GetValue("cover");
                                moreMessage.Author = (string)subMsg.GetValue("author");
                                moreMessage.CopyrightStat = (string)subMsg.GetValue("copyright_stat");
                                messages.Add(moreMessage);
                            }

                        }
                        continue;

                    case "62": //视频
                        aMessage.CdnVideoId = (string)msg.SelectToken("video_msg_ext_info.cdn_videoid");
                        aMessage.Thumb = (string)msg.SelectToken("video_msg_ext_info.thumb");
                        aMessage.VideoSrc = "https://mp.weixin.qq.com/mp/getcdnvideourl?__biz=" + biz + "&cdn_videoid=" +
                            aMessage.CdnVideoId + "&thumb=" + aMessage.Thumb + "&uin=" + "&key=" + key;
                        break;
                    default:
                        break;
                }

                messages.Add(aMessage);
            }

            // 删除搜狗本身携带的空数据
            List<BatchMessage> FinalMessages = new List<BatchMessage>();
            foreach (var m in messages)
            {
                if (m.Type == "49" && string.IsNullOrEmpty(m.ContentUrl))
                {

                }
                else
                {
                    FinalMessages.Add(m);
                }
            }

            return FinalMessages;


        }


        /// <summary>
        /// 获取文章html
        /// </summary>
        /// <param name="url">文章链接</param>
        /// <remarks>_get_gzh_article_text</remarks>
        /// <returns></returns>
        protected string _GetOfficialAccountArticleHtml(string url)
        {

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("host", "mp.weixin.qq.com");
            HttpHelper netHelper = new HttpHelper();
            return netHelper.Get(headers, url);


        }


        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        protected string _GetRelatedJson(string url, string title)
        {
            string related_req_url = "http://mp.weixin.qq.com/mp/getrelatedmsg?" + "url=" + url + "&title=" + title + "&uin=&key=&pass_ticket=&wxtoken=&devicetype=&clientversion=0&x5=0";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Host", "mp.weixin.qq.com");
            headers.Add("Referer", url);
            HttpHelper netHelper = new HttpHelper();
            var related_text = netHelper.Get(headers, url);
            try
            {
                JObject relateJson = JObject.Parse(related_text);
                string errMsg = "";
                int ret = (int)relateJson.SelectToken("base_resp.ret");
                if (relateJson.SelectToken("base_resp.errmsg") != null)
                {
                    errMsg = (string)relateJson.SelectToken("base_resp.errmsg");

                }
                else
                {
                    errMsg = "ret:" + ret;
                }

                if (ret != 0)
                {
                    logger.Error(errMsg);
                    throw new WechatSogouException();
                }
                return relateJson.ToString();

            }
            catch
            {
                return "";
            }


        }


        EncryptArgs _uinkeybiz(string keyword, string uin, string key, string biz, string pass_ticket, string msgid)
        {
            EncryptArgs encrpt = new EncryptArgs();
            if (string.IsNullOrEmpty(uin))
            {
                this.weChatCache.Update(keyword + "uin", uin, 36000);
                this.weChatCache.Update(keyword + "key", key, 36000);
                this.weChatCache.Update(keyword + "biz", biz, 36000);
                this.weChatCache.Update(keyword + "pass_ticket", pass_ticket, 36000);
                this.weChatCache.Update(keyword + "msgid", msgid, 36000);
            }
            else
            {
                uin = weChatCache.Get<object>(keyword + "uin").ToString();
                key = this.weChatCache.Get<object>(keyword + "key").ToString();
                biz = this.weChatCache.Get<object>(keyword + "biz").ToString();
                pass_ticket = this.weChatCache.Get<object>(keyword + "pass_ticket").ToString();
                msgid = this.weChatCache.Get<object>(keyword + "msgid").ToString();

                encrpt.uin = uin;
                encrpt.key = key;
                encrpt.biz = biz;
                encrpt.pass_ticket = pass_ticket;
                encrpt.msgid = msgid;
                encrpt.uin = uin;

            }
            return encrpt;
            //    def _uinkeybiz(self, keyword, uin= None, key = None, biz = None, pass_ticket = None, msgid = None):
            //if uin:
            //    self._cache.set(keyword + 'uin', uin, 36000)
            //    self._cache.set(keyword + 'key', key, 36000)
            //    self._cache.set(keyword + 'biz', biz, 36000)
            //    self._cache.set(keyword + 'pass_ticket', pass_ticket, 36000)
            //    self._cache.set(keyword + 'msgid', msgid, 36000)
            //else:
            //    uin = self._cache.get(keyword + 'uin')
            //    key = self._cache.get(keyword + 'key')
            //    biz = self._cache.get(keyword + 'biz')
            //    pass_ticket = self._cache.get(keyword + 'pass_ticket')
            //    msgid = self._cache.get(keyword + 'msgid')
            //    return uin, key, biz, pass_ticket, msgid

        }

        string _cache_history_session()
        {
            return "";
            //    def _cache_history_session(self, keyword, session= None):
            //if session:
            //    self._cache.set(keyword + 'session', session, 36000)
            //else:
            //    return self._cache.get(keyword + 'session')

        }



    }
}
