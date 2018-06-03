using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using WeGouSharp.Model;

namespace WeGouSharp
{

    class WechatSogouBasic
    {
        private readonly ILog _logger;

        int _tryCount;
        string _vcode_url = "";

        readonly WechatCache _weChatCache;
        
        public static List<string> UserAgents;


        protected WechatSogouBasic()
        {
            _weChatCache = new WechatCache(Config.CacheDir, 60 * 60);
            _logger = ServiceProviderAccessor.ServiceProvider.GetService(typeof(ILog)) as ILog;
            UserAgents = Config.Configuration.GetSection("UserAgent").Get<List<string>>();
        }


        /// <summary>
        /// 通过搜狗搜索指定关键字返回的文本
        /// </summary>
        /// <param name="name">搜索关键字</param>
        /// <param name="page">搜索的页数</param>
        /// <param name="tryTime"></param>
        /// <returns>返回的html string</returns>
        protected string _SearchAccount_Html(string name, int page = 1, int tryTime = 1)
        {
            string text = "";
            var headers = new WebHeaderCollection();
            var netHelper = new HttpHelper();
            name = WebUtility.UrlEncode(name);
            string requestUrl =
                $"http://weixin.sogou.com/weixin?query={name}&_sug_type_=&_sug_=n&type=1&page={page}&ie=utf8";
            //toconfirm if the link can not unlock for some incorrect code 

            try
            {
                text = tryTime > 5 ? "" : netHelper.Get(headers, requestUrl, "utf-8",true);
            }
            catch (WechatSogouVcodeException vCodeEx)
            {
               var unlockCode = netHelper.UnLock(false);

                //continute request after post vcode notice ref and request url
                var refParam = vCodeEx.VisittingUrl.Replace("http://weixin.sogou.com", "");
                refParam = WebUtility.UrlDecode(refParam);//部分encode，先全部还原源字符
                // AFTER VCODE  ,send request with unlock code by cookie          
                refParam = "http://weixin.sogou.com/antispider/?from=" + System.Web.HttpUtility.UrlEncode(refParam);

                headers.Add("referer", refParam);
                headers.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                tryTime++;
                text = tryTime > 5 ? "" : netHelper.VcodeJump(headers, requestUrl, "", true,unlockCode);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
            var browser = new HttpHelper();
            HttpHelper netHelper = new HttpHelper();
            var headers = new WebHeaderCollection();
            try
            {
                text = browser.Get(headers, requestUrl);
            }
            catch (Exception e) //todo should catch vcode exception
            { 
                if (e.Message == "weixin.sogou.com verification code")
                {
                    browser.UnLock(false);
                    headers.Add("referer", "http://weixin.sogou.com/antispider/?from=%2f" + this._vcode_url.Replace("http://", ""));
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
        protected string _GetRecentArticle_Html(string url)
        {

            var headers = new WebHeaderCollection {{"host", "mp.weixin.qq.com"}};
            var netHelper = new HttpHelper();
            string text = netHelper.Get(headers, url);
            _tryCount = 1;

            if (!text.Contains("为了保护你的网络安全，请输入验证码") && _tryCount <= 1) return text;
            
            
            netHelper.VerifyCodeForContinute(url, false);
            //解封后再次请求
            text = netHelper.Get(headers, url, "UTF-8", true);
            if (text.Contains("验证码有误"))
            {
                Console.WriteLine("验证时输入错误");
                if (_tryCount > 1)
                {
                    throw new WechatSogouVcodeFailException($"验证码识别错误 url:{url}");
                }
            }
            _GetRecentArticle_Html(url);
            return text;
        }


        /// <summary>
        /// 最近文章页  公众号信息
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected OfficialAccount _ResolveOfficialAccount(string htmlText, string url)
        {
            var officialAccount = new OfficialAccount {AccountPageurl = url};
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlText);
            var profileInfoArea = doc.DocumentNode.SelectSingleNode("//div[@class='profile_info_area']");
            officialAccount.ProfilePicture = profileInfoArea.SelectSingleNode("div[1]/span/img").GetAttributeValue("src", "");
            officialAccount.Name = profileInfoArea.SelectSingleNode("div[1]/div/strong/text()").InnerText.Trim();
            string wechatId = profileInfoArea.SelectSingleNode("div[1]/div/p/text()").InnerText.Trim();
            wechatId = wechatId.Length > 0 ? wechatId.Replace("微信号: ", "") : "";

            officialAccount.WeChatId = wechatId;
            officialAccount.Introduction = profileInfoArea.SelectSingleNode("ul/li[1]/div/text()").InnerText;
            string authInfo = profileInfoArea.SelectSingleNode("ul/li[2]/div/text()").InnerText;

            officialAccount.IsAuth = !string.IsNullOrEmpty(authInfo);

            string qrcode = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//*[@id='js_pc_qr_code_img']").GetAttributeValue("src", ""));

            qrcode = qrcode.Length > 0 ? "http://mp.weixin.qq.com" + qrcode : "";

            officialAccount.QrCode = qrcode;
            return officialAccount;

        }



        /// <summary>
        /// 从网页内容提取出最近文章内容（json）
        /// </summary>
        /// <param name="text"></param>
        /// 
        /// <returns>list of BatchMessage</returns>
        protected string _ExtracJson(string text)
        {
            string msglist = "";
            var reg = new Regex("var msgList =(.+?)};");
            var match = reg.Match(text);
            msglist = match.Groups[1].Value;

            msglist = msglist + "}";
            var msgdict = WebUtility.HtmlDecode(msglist);
            return msgdict;
        }


        /// <summary>
        /// 解析 公众号 群发消息
        /// </summary>
        /// <param name="jsonText"></param>
        /// <param name="encryp"></param>
        /// <returns></returns>
        protected List<BatchMessage> _ResolveBatchMessageFromJson(string jsonText, EncryptArgs encryp)
        {
            var messages = new List<BatchMessage>();
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
                        JObject appMsgExtInfo = (JObject)msg.GetValue("app_msg_ext_info");
                        string url = (string)appMsgExtInfo.GetValue("content_url");
                        if (!String.IsNullOrEmpty(url))
                        {
                            if (!url.Contains("http://mp.weixin.qq.com")) { url = "http://mp.weixin.qq.com" + url; }

                        }
                        else
                        {
                            url = "";
                        }
                        aMessage.Main = 1;
                        aMessage.Title = (string)appMsgExtInfo.GetValue("title");
                        aMessage.Digest = (string)appMsgExtInfo.GetValue("digest");
                        aMessage.FileId = (int)appMsgExtInfo.GetValue("fileid");
                        aMessage.ContentUrl = url;
                        aMessage.SourceUrl = (string)appMsgExtInfo.GetValue("source_url");
                        aMessage.Cover = (string)appMsgExtInfo.GetValue("cover");
                        aMessage.Author = (string)appMsgExtInfo.GetValue("author");
                        aMessage.CopyrightStat = (string)appMsgExtInfo.GetValue("copyright_stat");
                        messages.Add(aMessage);

                        if ((int)appMsgExtInfo.GetValue("is_multi") == 1)
                        {
                            JArray multiAppMsgItemList = (JArray)appMsgExtInfo.GetValue("multi_app_msg_item_list");
                            var moreMessage = new BatchMessage();
                            foreach (var jToken in multiAppMsgItemList)
                            {
                                var subMsg = (JObject) jToken;
                                url = (string)subMsg.GetValue("content_url");
                                if (!string.IsNullOrEmpty(url))
                                {
                                    if (!url.Contains("http://mp.weixin.qq.com")) 
                                    { url = "http://mp.weixin.qq.com" + url; }
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
            var finalMessages = new List<BatchMessage>();
            foreach (var m in messages)
            {
                if (m.Type == "49" && string.IsNullOrEmpty(m.ContentUrl))
                {

                }
                else
                {
                    finalMessages.Add(m);
                }
            }

            return finalMessages;


        }


        /// <summary>
        /// 获取文章html
        /// </summary>
        /// <param name="url">文章链接</param>
        /// <remarks>_get_gzh_article_text</remarks>
        /// <returns></returns>
        protected string _GetOfficialAccountArticleHtml(string url)
        {

            var headers = new WebHeaderCollection {{"host", "mp.weixin.qq.com"}};
            var netHelper = new HttpHelper();
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
            string relatedReqUrl = "http://mp.weixin.qq.com/mp/getrelatedmsg?" + "url=" + url + "&title=" + title + "&uin=&key=&pass_ticket=&wxtoken=&devicetype=&clientversion=0&x5=0";
            var headers = new WebHeaderCollection {{"Host", "mp.weixin.qq.com"}, {"Referer", url}};
            var netHelper = new HttpHelper();
            var relatedText = netHelper.Get(headers, url);
            try
            {
                JObject relateJson = JObject.Parse(relatedText);
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
                    _logger.Error(errMsg);
                    throw new WechatSogouException();
                }
                return relateJson.ToString();

            }
            catch
            {
                return "";
            }


        }


        EncryptArgs _uinkeybiz(string keyword, string uin, string key, string biz, string passTicket, string msgid)
        {
            var encrpt = new EncryptArgs();
            if (string.IsNullOrEmpty(uin))
            {
                _weChatCache.Update(keyword + "uin", uin, 36000);
                _weChatCache.Update(keyword + "key", key, 36000);
                _weChatCache.Update(keyword + "biz", biz, 36000);
                _weChatCache.Update(keyword + "pass_ticket", passTicket, 36000);
                _weChatCache.Update(keyword + "msgid", msgid, 36000);
            }
            else
            {
                uin = _weChatCache.Get<object>(keyword + "uin").ToString();
                key = _weChatCache.Get<object>(keyword + "key").ToString();
                biz = _weChatCache.Get<object>(keyword + "biz").ToString();
                passTicket = _weChatCache.Get<object>(keyword + "pass_ticket").ToString();
                msgid = _weChatCache.Get<object>(keyword + "msgid").ToString();

                encrpt.uin = uin;
                encrpt.key = key;
                encrpt.biz = biz;
                encrpt.pass_ticket = passTicket;
                encrpt.msgid = msgid;
                encrpt.uin = uin;

            }
            return encrpt;
           
        }

    }
}
