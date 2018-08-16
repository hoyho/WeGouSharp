using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeGouSharp.Model;
using WeGouSharp.Model.OriginalInfo;

namespace WeGouSharp.Core
{
    class WechatSogouBasic
    {
        private readonly ILog _logger;
        private readonly Browser _browser;
        private readonly IConfiguration _configuration;
        private int _tryCount;

        public static List<string> UserAgents;


        protected WechatSogouBasic(ILog logger, Browser browser, IConfiguration configuration)
        {
            _logger = logger;
            _browser = browser;
            UserAgents = configuration.GetSection("UserAgent").Get<List<string>>();
            _configuration = configuration;
        }


        /// <summary>
        /// 通过搜狗搜索指定关键字返回的文本
        /// </summary>
        /// <param name="name">搜索关键字</param>
        /// <param name="page">搜索的页数</param>
        /// <param name="tryTime"></param>
        /// <returns>返回的html string</returns>
        protected async Task<string> SearchAccountHtmlAsync(string name, int page = 1, int tryTime = 1)
        {
            string text = "";
            name = WebUtility.UrlEncode(name);
            string requestUrl =
                $"http://weixin.sogou.com/weixin?query={name}&_sug_type_=&_sug_=n&type=1&page={page}&ie=utf8";

            try
            {
                text = tryTime > 5 ? "" : await _browser.GetPageWithoutVcodeAsync(requestUrl);
            }
            catch (WechatSogouVcodeException vCodeEx)
            {
                var enableOnlineDecode = _configuration.GetSection("enableOnlineDecode").Get<bool>();
                await _browser.HandleSogouVcodeAsync(vCodeEx.VisittingUrl, enableOnlineDecode);
                tryTime++;
                text = tryTime > 5 ? "" : await _browser.GetPageWithoutVcodeAsync(requestUrl);
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
        protected async Task<string> SearchArticleHtmlAsync(string name, int page)
        {
            string requestUrl = "http://weixin.sogou.com/weixin?query=" + name + "&_sug_type_=&_sug_=n&type=2&page=" +
                                page + "&ie=utf8";
            string text = "";
            try
            {
                text = await _browser.GetPageWithoutVcodeAsync(requestUrl);
            }
            catch (WechatSogouVcodeException vCodeEx)
            {
                var enableOnlineDecode = _configuration.GetSection("enableOnlineDecode").Get<bool>();

                await _browser.HandleSogouVcodeAsync(vCodeEx.VisittingUrl, enableOnlineDecode);

                await _browser.GetPageWithoutVcodeAsync(requestUrl);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return text;
        }


        /// <summary>
        /// 获取最近文章页的文本
        /// </summary>
        /// <param name="url">最近文章页地址</param>
        /// <returns></returns>
        protected async Task<string> _GetRecentArticle_Html(string url)
        {
            string text = "";
            try
            {
                text = await _browser.GetPageWithoutVcodeAsync(url);

                return text;
            }
            catch (WechatSogouVcodeException ve)
            {
                Console.WriteLine(ve.ToString());
                var enableOnlineDecode = _configuration.GetSection("EnableOnlineDecode").Get<bool>();
                if (await _browser.HandleSogouVcodeAsync(ve.VisittingUrl, enableOnlineDecode))
                {
                }

                text = await _browser.GetPageWithoutVcodeAsync(url);
            }
            catch (WechatWxVcodeException vxEx)
            {
                Console.WriteLine(vxEx.ToString());
                var enableOnlineDecode = _configuration.GetSection("EnableOnlineDecode").Get<bool>();

                await _browser.HandleWxVcodeAsync(vxEx.VisittingUrl, enableOnlineDecode);
                text = await _browser.GetPageWithoutVcodeAsync(url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

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
            var officialAccount = new OfficialAccount { AccountPageurl = url };
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlText);
            var profileInfoArea = doc.DocumentNode.SelectSingleNode("//div[@class='profile_info_area']");
            officialAccount.ProfilePicture =
                profileInfoArea.SelectSingleNode("div[1]/span/img").GetAttributeValue("src", "");
            officialAccount.Name = profileInfoArea.SelectSingleNode("div[1]/div/strong/text()").InnerText.Trim();
            string wechatId = profileInfoArea.SelectSingleNode("div[1]/div/p/text()").InnerText.Trim();
            wechatId = wechatId.Length > 0 ? wechatId.Replace("微信号: ", "") : "";

            officialAccount.WeChatId = wechatId;
            officialAccount.Introduction = profileInfoArea.SelectSingleNode("ul/li[1]/div/text()").InnerText;
            string authInfo = profileInfoArea.SelectSingleNode("ul/li[2]/div/text()").InnerText;

            officialAccount.IsAuth = !string.IsNullOrEmpty(authInfo);

            string qrcode = WebUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//*[@id='js_pc_qr_code_img']")
                .GetAttributeValue("src", ""));

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
            var reg = new Regex("var msgList =(.+?)};");
            var match = reg.Match(text);
            var msglist = match.Groups[1].Value;

            msglist = msglist + "}";
            return msglist;
            //var msgdict = WebUtility.HtmlDecode(msglist);
            //return msgdict;
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

            var root = JsonConvert.DeserializeObject<MsgRoot>(jsonText);

            var msgs = HandleHtmlCode(root.list);
            msgs.ForEach(m =>
            {
                var message = new BatchMessage()
                {
                    Author = m.app_msg_ext_info.author,
                    Meaasgeid = m.comm_msg_info.id,
                    SendDate = m.comm_msg_info.datetime.ToString(),
                    Type = m.comm_msg_info.type.ToString()
                };

                switch (message.Type)
                {
                    case "1": //文字
                        message.Content = m.comm_msg_info.content;
                        break;
                    case "3": //图片
                        message.ImageUrl =
                            $"https://mp.weixin.qq.com/mp/getmediadata?biz={biz}&type=img&mode=small&msgid={message.Meaasgeid}&uin=uin&key={key}";
                        break;
                    case "34": //音频
                        message.PlayLength = m.voice_msg_ext_info.play_length;
                        message.FileId = m.voice_msg_ext_info.fileid;
                        message.AudioSrc =
                            $"https://mp.weixin.qq.com/mp/getmediadata?biz={biz}&type=voice&msgid={message.Meaasgeid}&uin={uin}&key={key}";
                        break;
                    case "49": //图文
                        message.ContentUrl = m.app_msg_ext_info.content_url.Contains("http://mp.weixin.qq.com")
                            ? m.app_msg_ext_info.content_url
                            : "http://mp.weixin.qq.com" + m.app_msg_ext_info.content_url;
                        message.Main = 1;
                        message.Title = m.app_msg_ext_info.title;
                        message.Digest = m.app_msg_ext_info.digest;
                        message.FileId = m.app_msg_ext_info.fileid;
                        message.SourceUrl = m.app_msg_ext_info.source_url;
                        message.Cover = m.app_msg_ext_info.cover;
                        message.Author = m.app_msg_ext_info.author;
                        message.CopyrightStat = m.app_msg_ext_info.copyright_stat.ToString();
                        messages.Add(message);
                        if (m.app_msg_ext_info.is_multi == 1) //源群发消息文章还包含多个副文章
                        {
                            var subMsgs = ParseSubArticle(m.app_msg_ext_info.multi_app_msg_item_list);
                            subMsgs.ForEach(subMsg => subMsg.SendDate = message.SendDate);
                            messages.AddRange(subMsgs);
                        }

                        break;
                    case "62": //视频
                        message.CdnVideoId = m.video_msg_ext_info.cdn_videoid;
                        message.Thumb = m.video_msg_ext_info.thumb;
                        message.VideoSrc =
                            $"https://mp.weixin.qq.com/mp/getcdnvideourl?biz={biz}&cdn_videoid={message.CdnVideoId}&thumb={message.Thumb}&uin=&key={key}";
                        break;
                }
            });


            // 删除搜狗本身携带的空数据,(type=49以及contentUrl为空的是无效数据)
            var finalMessages = messages.Where(msg => !(msg.Type == "49"
                                                        && string.IsNullOrEmpty(msg.ContentUrl))).ToList();

            return finalMessages;
        }


        /// <summary>
        /// 获取文章html
        /// </summary>
        /// <param name="url">文章链接</param>
        /// <remarks>_get_gzh_article_text</remarks>
        /// <returns></returns>
        protected async Task<string> _GetOfficialAccountArticleHtml(string url)
        {
            return await _browser.GetPageAsync(url);
        }


        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        protected async Task<string> _GetRelatedJson(string url, string title)
        {
            var relatedText = await _browser.GetPageAsync(url);
            try
            {
                JObject relateJson = JObject.Parse(relatedText);
                string errMsg;
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


        /// <summary>
        /// 把源MultiAppMsgItemList列表(副图文)转换为群发消息对象BatchMessage
        /// </summary>
        /// <param name="multiAppMsgItemLists"></param>
        /// <returns></returns>
        private List<BatchMessage> ParseSubArticle(List<MultiAppMsgItemList> multiAppMsgItemLists)
        {
            var articleList = new List<BatchMessage>();
            foreach (var msg in multiAppMsgItemLists)
            {
                articleList.Add(new BatchMessage
                {
                    ContentUrl = msg.content_url.Contains("http://mp.weixin.qq.com")
                        ? msg.content_url
                        : "http://mp.weixin.qq.com" + msg.content_url,
                    Title = msg.title,
                    Digest = msg.digest,
                    FileId = msg.fileid,
                    SourceUrl = msg.source_url,
                    Cover = msg.cover,
                    Author = msg.author,
                    CopyrightStat = msg.copyright_stat.ToString()
                });
            }

            return articleList;
        }


        /// <summary>
        /// 把所有string属性都进行htmldecode,还原原始信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Msg> HandleHtmlCode(List<Msg> list)
        {
            var results = new List<Msg>();
            foreach (var element in list)
            {
                element.app_msg_ext_info.content = WebUtility.HtmlDecode(element.app_msg_ext_info.content);
                element.app_msg_ext_info.content_url = WebUtility.HtmlDecode(element.app_msg_ext_info.content_url);
                element.app_msg_ext_info.cover = WebUtility.HtmlDecode(element.app_msg_ext_info.cover);
                element.app_msg_ext_info.digest = WebUtility.HtmlDecode(element.app_msg_ext_info.digest);
                element.app_msg_ext_info.play_url = WebUtility.HtmlDecode(element.app_msg_ext_info.play_url);
                element.app_msg_ext_info.source_url = WebUtility.HtmlDecode(element.app_msg_ext_info.source_url);
                element.app_msg_ext_info.title = WebUtility.HtmlDecode(element.app_msg_ext_info.title);

                element.app_msg_ext_info.multi_app_msg_item_list.ForEach(sub =>
                {
                    sub.content = WebUtility.HtmlDecode(sub.content);
                    sub.content_url = WebUtility.HtmlDecode(sub.content_url);
                    sub.cover = WebUtility.HtmlDecode(sub.cover);
                    sub.digest = WebUtility.HtmlDecode(sub.digest);
                    sub.play_url = WebUtility.HtmlDecode(sub.play_url);
                    sub.source_url = WebUtility.HtmlDecode(sub.source_url);
                    sub.title = WebUtility.HtmlDecode(sub.title);
                });
                results.Add(element);
            }

            return results;
        }
    }
}