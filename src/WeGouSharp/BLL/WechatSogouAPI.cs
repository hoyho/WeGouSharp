using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeGouSharp.Model;

namespace WeGouSharp
{
    /// <inheritdoc />
    internal class WechatSogouAPI : WechatSogouBasic
    {
        readonly ILog _logger;

        public WechatSogouAPI(ILog logger, Browser browser) : base(logger, browser)
        {
            _logger = logger;
        }

        /// <summary>
        /// 搜索公众号
        /// </summary>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="page">第几页</param>
        /// <returns></returns>
        public async Task<List<OfficialAccount>> SearchOfficialAccountAsync(string keyword, int page = 1)
        {
            var accountList = new List<OfficialAccount>();

            //var text = this._SearchAccount_Html(keyword, page);
            var text = await SearchAccountHtmlAsync(keyword, page);
            var pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            var targetArea = pageDoc.DocumentNode.SelectNodes("//ul[@class='news-list2']/li");
            if (targetArea == null) return null;
            foreach (var node in targetArea)
            {
                var accountInfo = new OfficialAccount();
                try
                {
                    //链接中包含了&amp; html编码符，要用htmdecode，不是urldecode
                    accountInfo.AccountPageurl =
                        WebUtility.HtmlDecode(node.SelectSingleNode("div/div[@class='img-box']/a")
                            .GetAttributeValue("href", ""));
                    //accountInfo.ProfilePicture = node.SelectSingleNode("div/div[1]/a/img").InnerHtml;
                    accountInfo.ProfilePicture = WebUtility.HtmlDecode(node
                        .SelectSingleNode("div/div[@class='img-box']/a/img").GetAttributeValue("src", ""));
                    accountInfo.Name = node.SelectSingleNode("div/div[2]/p[1]").InnerText.Trim()
                        .Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "");
                    accountInfo.WeChatId = node.SelectSingleNode("div/div[2]/p[2]/label").InnerText.Trim();
                    accountInfo.QrCode =
                        WebUtility.HtmlDecode(node.SelectSingleNode("div/div[3]/span/img")
                            .GetAttributeValue("src", ""));
                    accountInfo.Introduction = node.SelectSingleNode("dl[1]/dd").InnerText.Trim()
                        .Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "");
                    //早期的账号认证和后期的认证显示不一样？，对比 bitsea 和 NUAA_1952 两个账号
                    //现在改为包含该script的即认证了
                    accountInfo.IsAuth = node.InnerText.Contains("document.write(authname('2'))");
                    accountList.Add(accountInfo);
                }
                catch (Exception e)
                {
                    _logger.Warn(e);
                }
            }

            return accountList;
        }


        /// <summary>
        /// 获取公众号微信号wechatid的信息 (实际上就是搜索公众号id，获取第一个结果)
        /// wechatid: 公众号id
        /// 因为wechatid唯一确定，所以第一个就是要搜索的公众号
        /// 
        /// </summary>
        /// <param name="wechatid"></param>
        /// <returns></returns>
        public async Task<OfficialAccount> GetAccountInfoByIdAsync(string wechatid)
        {
            var info = (await SearchOfficialAccountAsync(wechatid, 1))?.FirstOrDefault(); //可能为空
            return info;
        }


        /// <summary>
        /// 搜索微信文章
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Article> SearchArticle(string keyword, int page = 1)
        {
            var articleList = new List<Article>();
            string text = this._SearchArticle_Html(keyword, page);
            var pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            //todo
            var targetArea = pageDoc.DocumentNode.SelectNodes("//ul[@class='news-list']/li");
            foreach (var node in targetArea)
            {
                try
                {
                    string url =
                        WebUtility.HtmlDecode(node.SelectSingleNode("div[2]/h3/a").GetAttributeValue("href", ""));
                    string title;
                    var imgs = new List<string>();
                    string brief;
                    string time;
                    var account = new OfficialAccount();


                    if (!string.IsNullOrEmpty(url))
                    {
                        title = node.SelectSingleNode("div[2]/h3/a").InnerText;
                        string img =
                            WebUtility.HtmlDecode(node.SelectSingleNode("div[1]/a/img").GetAttributeValue("src", ""));
                        imgs.Add(img);
                        brief = node.SelectSingleNode("div[2]/p").InnerHtml;
                        time = node.SelectSingleNode("div[2]/div/span/script/text()").InnerHtml;
                        if (node.SelectSingleNode("div[@class='txt-box']/div[@class='s-p']/a") != null)
                        {
                            account.IsAuth = Convert.ToBoolean(Convert.ToInt16(node
                                .SelectSingleNode("div[@class='txt-box']/div[@class='s-p']/a")
                                .GetAttributeValue("data-isv", "")));
                        }

                        account.AccountPageurl = WebUtility.HtmlDecode(node
                            .SelectSingleNode("div[@class='txt-box']/h3/a").GetAttributeValue("href", ""));
                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.ProfilePicture = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a")
                                .GetAttributeValue("data-headimage", ""));
                        }

                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.Name = node.SelectSingleNode("div/div[2]/a").InnerText;
                        }
                    }
                    else
                    {
                        url = WebUtility.HtmlDecode(node.SelectSingleNode("div/h3/a").GetAttributeValue("href", ""));
                        title = node.SelectSingleNode("div/h3/a").InnerText;
                        var spansNodeCollection = node.SelectNodes("div/div[1]/a");
                        foreach (var span in spansNodeCollection)
                        {
                            string img = WebUtility.HtmlDecode(span.SelectSingleNode("span/img/@src").InnerText);
                            if (!string.IsNullOrEmpty(img))
                            {
                                imgs.Add(img);
                            }
                        }

                        brief = node.SelectSingleNode("div/p").InnerText;
                        time = node.SelectSingleNode("div/div[2]/span/script/text()").InnerText;
                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.IsAuth = Convert.ToBoolean(node.SelectSingleNode("div/div[2]/a")
                                .GetAttributeValue("data-isv", ""));
                        }

                        account.AccountPageurl =
                            WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a").GetAttributeValue("href", ""));
                        account.ProfilePicture = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a")
                            .GetAttributeValue("data-headimage", ""));
                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.Name = node.SelectSingleNode("div/div[2]/a").InnerText;
                        }
                    }


                    title = string.IsNullOrEmpty(title)
                        ? ""
                        : title.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "")
                            .Replace("<em>", "").Replace("</em>", "");

                    brief = string.IsNullOrEmpty(brief)
                        ? ""
                        : brief.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "").Replace("<em>", "")
                            .Replace("</em>", "");

                    var timeRegex = new Regex(@"timeConvert\('(?(time)<1>(\d+))'\)");
                    time = timeRegex.Match(time).Groups[1].Value;

                    var article = new Article
                    {
                        Title = title,
                        Brief = brief,
                        Url = url,
                        Imgs = imgs,
                        Time = time,
                        OfficialAccount = account
                    };
                    articleList.Add(article);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }

            return articleList;
        }


        /// <summary>
        /// 根据accountPageUrl或者wechatid或者wechatname 解析最近文章页并解析历史消息记录（只需要指明一个参数即可）
        /// </summary>
        /// <param name="accountPageUrl">最近文章地址</param>
        /// <param name="wechatId">微信号</param>
        /// <param name="wechatName">微信昵称(不推荐，因为不唯一)</param>
        /// <remarks> 最保险的做法是提供url或者wechatid</remarks>
        /// <returns> list of batchMessage 一定含有字段qunfa_id,datetime,type 当type不同时，含有不同的字段，具体见文档</returns>
        public async Task<List<BatchMessage>> GetOfficialAccountMessagesAsync(string accountPageUrl = "", string wechatId = "",
            string wechatName = "")
        {
            string htmlStr = "";
            if (!string.IsNullOrEmpty(accountPageUrl))
            {
                htmlStr = _GetRecentArticle_Html(accountPageUrl);
            }
            else if (!string.IsNullOrEmpty(wechatId))
            {
                var account = await GetAccountInfoByIdAsync(wechatId);
                accountPageUrl = account.AccountPageurl;
                htmlStr = _GetRecentArticle_Html(accountPageUrl);
            }
            else if (!string.IsNullOrEmpty(wechatName))
            {
                var account = await GetAccountInfoByIdAsync(wechatName);
                accountPageUrl = account.RecentArticleUrl;
                htmlStr = _GetRecentArticle_Html(accountPageUrl);
            }

            var encry = new EncryptArgs();
            var articleJsonString = this._ExtracJson(htmlStr);
            return this._ResolveBatchMessageFromJson(articleJsonString, encry);
        }


        /// <summary>
        /// 根据accountPageUrl，或者wechatid或者 wechatname获取公众号相关信息及已发消息（视频，语音或者文章）json格式
        /// </summary>
        /// <param name="accountPageUrl"></param>
        /// <param name="wechatId"></param>
        /// <param name="wechatName"></param>
        /// <remarks>get_gzh_message_and_info</remarks>
        /// <returns>公众号相关信息及已发消息(json)</returns>
        public async Task<string> GetOfficialAccountInfoAndMessagesAsync(string accountPageUrl = "", string wechatId = "",
            string wechatName = "")
        {
            string text = "";
            string url = "";
            if (!string.IsNullOrEmpty(accountPageUrl))
            {
                url = accountPageUrl;
                text = _GetRecentArticle_Html(url);
            }
            else if (!string.IsNullOrEmpty(wechatId))
            {
                var officialAccount = await GetAccountInfoByIdAsync(wechatId);
                url = officialAccount.AccountPageurl;
                text = _GetRecentArticle_Html(url);
            }
            else if (!string.IsNullOrEmpty(wechatName))
            {
                var officialAccount = await GetAccountInfoByIdAsync(wechatName);
                url = officialAccount.AccountPageurl;
                text = _GetRecentArticle_Html(url);
            }

            var encryp = new EncryptArgs();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject
            (
                new
                {
                    OfficialAccount = _ResolveOfficialAccount(text, url),
                    Message = _ResolveBatchMessageFromJson(_ExtracJson(text), encryp)
                }
                ,
                Newtonsoft.Json.Formatting.Indented
            );
            return json;
        }


        /// <summary>
        /// 从（临时）文章页抽取正文部分
        /// </summary>
        /// <param name="url"></param>
        /// <param name="articlePageHtml"></param>
        /// <remarks>deal_article_content</remarks>
        /// <returns></returns>
        public string ExtractArticleMain(string url, string articlePageHtml = "")
        {
            if (!string.IsNullOrEmpty(articlePageHtml)) //优先使用articlePageHtml
            {
                //pass
            }
            else if (!string.IsNullOrEmpty(url))
            {
                articlePageHtml = _GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new WechatSogouException()
                {
                    Source = "ExtractArticleMain parameter should not be null"
                };
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(articlePageHtml);
            string bodyContent = "";
            try
            {
                bodyContent = doc.DocumentNode.SelectSingleNode("//*[@id='js_content']").InnerHtml;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            bodyContent = "<div class=\"rich_media_content \" id=\"js_content\">" + bodyContent + "</div>";

            return bodyContent;
        }


        /// <summary>
        /// 获取相似文章，此接口已经过期，待修复
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <remarks>deal_article_related</remarks>
        /// <returns>相似文章（json格式）</returns>
        [Obsolete]
        public string GetRelatedArticleJson(string url, string title)
        {
            return _GetRelatedJson(url, title);
        }


        /// <summary>
        /// 请求文章相关评论（sogou此接口已经过期，待更新）
        /// </summary>
        /// <param name="url">文章链接</param>
        /// <param name="articlePageHtml">文章页html</param>
        /// <remarks>deal_article_comment</remarks>
        /// <returns></returns>
        [Obsolete]
        public string RequireArticleComment(string url, string articlePageHtml = "")
        {
            if (string.IsNullOrEmpty(articlePageHtml))
            {
                //
            }
            else if (string.IsNullOrEmpty(url))
            {
                articlePageHtml = _GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new Exception("deal_content need param url or text");
            }


            var reg = new Regex("window.sg_data={(.*?)}");
            var match = reg.Match(articlePageHtml ?? "");
            string sgData = match.Groups[0].Value;
            sgData = "{" + sgData.Replace("\r\n", "").Replace(" ", "") + "}";

            reg = new Regex("{src:\"(.*?)\",ver:\"(.*?)\",timestamp:\"(.*?)\",signature:\"(.*?)\"}");
            var sgDataMatch = reg.Match(sgData);
            string commentReqUrl = "http://mp.weixin.qq.com/mp/getcomment?src=" + sgDataMatch.Groups[0].Value +
                                   "&ver=" + sgDataMatch.Groups[1].Value +
                                   "&timestamp=" + sgDataMatch.Groups[2] + "&signature=" + sgDataMatch.Groups[3].Value +
                                   "&uin=&key=&pass_ticket=&wxtoken=&devicetype=&clientversion=0&x5=0";

            var headers = new WebHeaderCollection();
            headers.Add("host", "mp.weixin.qq.com");
            headers.Add("referer", "http://mp.weixin.qq.com");

            var netHelper = new HttpHelper();
            string commentText = netHelper.Get(headers, commentReqUrl);
            JObject commentJson = new JObject();
            try
            {
                commentJson = JObject.Parse(commentText);
                int ret = (int)commentJson.SelectToken("base_resp.ret");
                string errorMsg = "";
                if (commentJson.SelectToken("base_resp.errmsg") != null)
                {
                    errorMsg = (string)commentJson.SelectToken("base_resp.errmsg");
                }
                else
                {
                    errorMsg = "ret:" + ret;
                }

                if (ret != 0)
                {
                    _logger.Error(errorMsg);
                    throw new WechatSogouException();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }


            return commentJson.ToString();
        }


        /// <summary>
        /// 请求文章阅读原文（sogou此接口已经过期，待更新）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="articlePageHtml"></param>
        /// <returns>"阅读原文(的链接)"</returns>
        [Obsolete]
        string RequireReadOriginal(string url, string articlePageHtml = "")
        {
            string originalLink = "";
            if (!string.IsNullOrEmpty(articlePageHtml))
            {
                //
            }
            else if (!string.IsNullOrEmpty(url))
            {
                articlePageHtml = _GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new Exception("dearlArticleOriginal need param url or text");
            }

            try
            {
                var reg = new Regex("var msg_link = \"(.*?)\";  ");
                var match = reg.Match(articlePageHtml);
                originalLink = match.Groups[0].Value.Replace("amp;", "");
            }
            catch (Exception e)
            {
                if (articlePageHtml.Contains("系统出错"))
                {
                    _logger.Info("系统出错 - 链接问题，正常");
                }
                else if (articlePageHtml.Contains("此内容因违规无法查看"))
                {
                    _logger.Info("此内容因违规无法查看 - 剔除此类文章");
                }
                else
                {
                    _logger.Error(e);
                    if (!string.IsNullOrEmpty(url))
                    {
                        _logger.Error(url);
                    }
                    else
                    {
                        var reg = new Regex("<title>(.*?)</title>");
                        var match = reg.Match(articlePageHtml);
                        _logger.Error(match.Groups[0].Value);
                    }
                }

                throw new Exception();
            }


            return originalLink;
        }


        /// <summary>
        /// "获取微信搜狗搜索关键词联想
        /// </summary>
        /// <param name="keyWord"></param>
        /// <remarks>get_sugg</remarks>
        /// <returns>sugg: 联想关键词数组</returns>
        public string[] GetSuggestKeyWords(string keyWord)
        {
            string[] suggArray;
            string url = "http://w.sugg.sogou.com/sugg/ajaj_json.jsp?key=" + keyWord + "&type=wxpub&pr=web";
            var netHelper = new HttpHelper();
            var headers = new WebHeaderCollection();
            var text = netHelper.Get(headers, url, "default");
            string kwPatten = @"\[""" + keyWord + @""",\[(.*?)\]"; //match: \["b2c",\[(.*?)\]
            var regex = new Regex(kwPatten);
            try
            {
                var match = regex.Match(text);
                string result = match.Groups["0"].Value;
                suggArray = result.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
            }
            catch (Exception e)
            {
                _logger.Error("sugg refind error", e);
                throw new Exception("sugg refind error");
            }

            return suggArray;
        }


        /// <summary>
        /// 获取首页热词，排行和热度
        /// </summary>
        /// <returns></returns>
        public List<HotWord> GetTopWords()
        {
            string url = "http://weixin.sogou.com/";
            var headers =
                new WebHeaderCollection
                {
                    {"Host", "weixin.sogou.com"},
                    {"Referer", "http://weixin.sogou.com/"}
                };
            var netHelper = new HttpHelper();
            string text = netHelper.Get(headers, url, "UTF-8");

            var pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            var targetArea = pageDoc.DocumentNode.SelectNodes("//*[@id='topwords']/li");
            var listTopWords = new List<HotWord>();

            var hotWord = new HotWord();
            foreach (var li in targetArea)
            {
                try
                {
                    hotWord.Rank = Convert.ToInt16(li.SelectSingleNode("i").InnerText);
                    hotWord.Word = li.SelectSingleNode("a").InnerText;
                    hotWord.JumpLink = li.SelectSingleNode("a").GetAttributeValue("href", "");
                    hotWord.HotDegree = Convert.ToInt16(li.SelectSingleNode("span/span").GetAttributeValue("style", "")
                        .Replace("width:", "").Replace("%", "").Trim());
                    listTopWords.Add(hotWord);
                }
                catch (Exception e)
                {
                    _logger.Debug(e);
                }
            }

            return listTopWords;
        }


        /// <summary>
        /// GetArticleByCategoryIndex
        /// </summary>
        /// <param name="categoryIndex">从0开始，首页分类，热门0, 推荐:1,段子手:2,养生堂:3,私房话:4  范围0-20 ?</param>
        /// <param name="page">页，从0开始</param>
        /// <remarks>get_recent_article_url_by_index_single</remarks>
        /// <returns></returns>
        public List<Article> GetArticleByCategoryIndex(int categoryIndex, int page)
        {
            string pageStr = ""; // "pc_" + page;
            if (page == 0)
            {
                pageStr = "pc_" + categoryIndex; //分类N第0页格式为xxxx/pc/pc_N/pc_N.html
            }
            else
            {
                pageStr = page.ToString();
            }

            //http://weixin.sogou.com/pcindex/pc/pc_4/pc_4.html //分类4第0页
            //http://weixin.sogou.com/pcindex/pc/pc_2/1.html //分类2第1页
            //http://weixin.sogou.com/pcindex/pc/pc_3/2.html //分类3第2页

            string url = "http://weixin.sogou.com/pcindex/pc/pc_" + categoryIndex + '/' + pageStr + ".html";
            var headers = new WebHeaderCollection
            {
                {"Host", "weixin.sogou.com"},
                {"Referer", "http://weixin.sogou.com/"},
                {"Accept", "*/*"}
            };
            var netHelper = new HttpHelper();
            string text = netHelper.Get(headers, url, "UTF-8");

            var pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            string targetXpath = "";
            targetXpath = page == 0 ? "//ul[@class='news-list']/li" : "li";

            var targetArea = pageDoc.DocumentNode.SelectNodes(targetXpath);

            var listArticle = new List<Article>();

            if (targetArea == null) return null;

            foreach (var li in targetArea)
            {
                var article = new Article() { Imgs = new List<string>() };
                var account = new OfficialAccount();
                try
                {
                    article.Title = li.SelectSingleNode("div[2]/h3/a").InnerText;
                    article.Url = li.SelectSingleNode("div[1]/a").GetAttributeValue("href", "");
                    article.Brief =
                        WebUtility.HtmlDecode(li.SelectSingleNode("div[2]/p[@class='txt-info']").InnerText);
                    string coverImg = li.SelectSingleNode("div[1]/a/img").GetAttributeValue("src", "");
                    if (!string.IsNullOrEmpty(coverImg))
                    {
                        article.Imgs.Add(coverImg);
                    }

                    article.Time = li.SelectSingleNode("div[2]/div/span").GetAttributeValue("t", "");
                    article.ArticleListUrl = li.SelectSingleNode("div[2]/div/a").GetAttributeValue("href", "");

                    account.AccountPageurl = li.SelectSingleNode("div[2]/div/a").GetAttributeValue("href", "");
                    account.Name = li.SelectSingleNode("div[2]/div/a").InnerText;
                    string isV = li.SelectSingleNode("div[2]/div/a").GetAttributeValue("data-isv", "");
                    account.IsAuth = isV == "1";
                    account.ProfilePicture =
                        li.SelectSingleNode("div[2]/div/a").GetAttributeValue("data-headimage", "");

                    article.OfficialAccount = account;
                    listArticle.Add(article);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }


            return listArticle;
        }


        /// <summary>
        ///获取首页推荐文章公众号最近文章地址，所有分类，maxPage个页数
        /// </summary>
        /// <remarks>get_recent_article_url_by_index_all</remarks>
        /// <returns></returns>
        public List<Article> GetAllRecentArticle(int maxPage)
        {
            var listArticles = new List<Article>();

            for (int cateIndex = 0; cateIndex < 20; cateIndex++)
            {
                int pageIndex = 0;
                var articles = GetArticleByCategoryIndex(cateIndex, pageIndex);
                while (pageIndex < maxPage)
                {
                    listArticles.AddRange(articles);
                    pageIndex += 1;
                    articles = GetArticleByCategoryIndex(cateIndex, pageIndex);
                }
            }

            return listArticles;
        }


    }
}