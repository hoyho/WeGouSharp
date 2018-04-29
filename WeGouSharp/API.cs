using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WeGouSharpPlus.Model;

namespace WeGouSharpPlus
{
    class API
    {
        ILog logger = LogManager.GetLogger(typeof(Program));
    }


    class WechatSogouAPI : WechatSogouBasic
    {

        ILog logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// 搜索公众号
        /// </summary>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="page">第几页</param>
        /// <returns></returns>
        public List<OfficialAccount> SearchOfficialAccount(string keyword, int page = 1)
        {
            List<OfficialAccount> accountList = new List<OfficialAccount>();

            string text = this._SearchAccount_Html(keyword, page);
            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            HtmlNodeCollection targetArea = pageDoc.DocumentNode.SelectNodes("//ul[@class='news-list2']/li");
            if (targetArea != null)
            {
                foreach (HtmlNode node in targetArea)
                {
                    try
                    {
                        OfficialAccount accountInfo = new OfficialAccount();

                        //链接中包含了&amp; html编码符，要用htmdecode，不是urldecode
                        accountInfo.AccountPageurl = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[@class='img-box']/a").GetAttributeValue("href", ""));
                        //accountInfo.ProfilePicture = node.SelectSingleNode("div/div[1]/a/img").InnerHtml;
                        accountInfo.ProfilePicture = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[@class='img-box']/a/img").GetAttributeValue("src", ""));
                        accountInfo.Name = node.SelectSingleNode("div/div[2]/p[1]").InnerText.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "");
                        accountInfo.WeChatId = node.SelectSingleNode("div/div[2]/p[2]/label").InnerText.Trim();
                        accountInfo.QrCode = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[3]/span/img").GetAttributeValue("src", ""));
                        accountInfo.Introduction = node.SelectSingleNode("dl[1]/dd").InnerText.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "");
                        //早期的账号认证和后期的认证显示不一样？，对比 bitsea 和 NUAA_1952 两个账号
                        //现在改为包含该script的即认证了
                        if (node.InnerText.Contains("document.write(authname('2'))"))
                        {
                            accountInfo.IsAuth = true;
                        }
                        else
                        {
                            accountInfo.IsAuth = false;
                        }
                        accountList.Add(accountInfo);
                    }
                    catch (Exception e)
                    {
                        logger.Warn(e);
                    }


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
        public OfficialAccount GetAccountInfoById(string wechatid)
        {
            OfficialAccount info = this.SearchOfficialAccount(wechatid, 1).FirstOrDefault(); //可能为空
            return info;
        }




        /// <summary>
        /// 搜索微信文章
        /// </summary>
        /// <param name="name"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Article> SearchArticle(string keyword, int page = 1)
        {
            List<Article> articleList = new List<Article>();
            string text = this._SearchArticle_Html(keyword, page);
            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            //todo
            HtmlNodeCollection targetArea = pageDoc.DocumentNode.SelectNodes("//ul[@class='news-list']/li");
            foreach (HtmlNode node in targetArea)
            {
                try
                {
                    string url = WebUtility.HtmlDecode(node.SelectSingleNode("div[2]/h3/a").GetAttributeValue("href", ""));
                    string title;
                    List<string> imgs = new List<string>();
                    string brief;
                    string time;
                    OfficialAccount account = new OfficialAccount();


                    if (!string.IsNullOrEmpty(url))
                    {
                        title = node.SelectSingleNode("div[2]/h3/a").InnerText;
                        string img = WebUtility.HtmlDecode(node.SelectSingleNode("div[1]/a/img").GetAttributeValue("src", ""));
                        imgs.Add(img);
                        brief = node.SelectSingleNode("div[2]/p").InnerHtml;
                        time = node.SelectSingleNode("div[2]/div/span/script/text()").InnerHtml;
                        if (node.SelectSingleNode("div[@class='txt-box']/div[@class='s-p']/a") != null)
                        {
                            account.IsAuth = Convert.ToBoolean(Convert.ToInt16(node.SelectSingleNode("div[@class='txt-box']/div[@class='s-p']/a").GetAttributeValue("data-isv", "")));
                        }

                        account.AccountPageurl = WebUtility.HtmlDecode(node.SelectSingleNode("div[@class='txt-box']/h3/a").GetAttributeValue("href", ""));
                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.ProfilePicture = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a").GetAttributeValue("data-headimage", ""));
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
                        HtmlNodeCollection spansNodeCollection = node.SelectNodes("div/div[1]/a");
                        foreach (HtmlNode span in spansNodeCollection)
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
                            account.IsAuth = Convert.ToBoolean(node.SelectSingleNode("div/div[2]/a").GetAttributeValue("data-isv", ""));
                        }
                        account.AccountPageurl = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a").GetAttributeValue("href", ""));
                        account.ProfilePicture = WebUtility.HtmlDecode(node.SelectSingleNode("div/div[2]/a").GetAttributeValue("data-headimage", ""));
                        if (node.SelectSingleNode("div/div[2]/a") != null)
                        {
                            account.Name = node.SelectSingleNode("div/div[2]/a").InnerText;
                        }

                    }



                    if (!string.IsNullOrEmpty(title))
                    {
                        title = title.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "").Replace("<em>", "").Replace("</em>", ""); ;
                    }
                    else
                    {
                        title = "";
                    }

                    if (!string.IsNullOrEmpty(brief))
                    {
                        brief = brief.Trim().Replace("<!--red_beg-->", "").Replace("<!--red_end-->", "").Replace("<em>", "").Replace("</em>", "");

                    }
                    else
                    {
                        brief = "";
                    }

                    Regex TimeRegex = new Regex(@"timeConvert\('(?<1>(\d+))'\)");
                    var m = TimeRegex.Match(time);
                    time = TimeRegex.Match(time).Groups[1].Value;

                    Article article = new Article();
                    article.Title = title;
                    article.Brief = brief;
                    article.Url = url;
                    article.Imgs = imgs;
                    article.Time = time;
                    article.officialAccount = account;
                    articleList.Add(article);

                }
                catch (Exception e)
                {
                    logger.Error(e);
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
        public List<BatchMessage> GetOfficialAccountMessages(string accountPageUrl = "", string wechatId = "", string wechatName = "")
        {
            string htmlStr = "";
            if (!string.IsNullOrEmpty(accountPageUrl))
            {
                htmlStr = this._GetRecentArticle_Html(accountPageUrl);
            }
            else if (!string.IsNullOrEmpty(wechatId))
            {

                var account = this.GetAccountInfoById(wechatId);
                accountPageUrl = account.AccountPageurl;
                htmlStr = this._GetRecentArticle_Html(accountPageUrl);
            }
            else if (!string.IsNullOrEmpty(wechatName))
            {
                var account = this.GetAccountInfoById(wechatName);
                accountPageUrl = account.RecentArticleUrl;
                htmlStr = this._GetRecentArticle_Html(accountPageUrl);
            }

            EncryptArgs encry = new EncryptArgs();
            string articleJsonString = this._ExtracJson(htmlStr);
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
        public string GetOfficialAccountInfoAndMessages(string accountPageUrl = "", string wechatId = "", string wechatName = "")
        {

            string text = "";
            string url = "";
            if (!string.IsNullOrEmpty(accountPageUrl))
            {
                url = accountPageUrl;
                text = this._GetRecentArticle_Html(url);
            }
            else if (!string.IsNullOrEmpty(wechatId))
            {
                var officialAccount = this.GetAccountInfoById(wechatId);
                url = officialAccount.AccountPageurl;
                text = this._GetRecentArticle_Html(url);
            }
            else if (!string.IsNullOrEmpty(wechatName))
            {
                var officialAccount = this.GetAccountInfoById(wechatName);
                url = officialAccount.AccountPageurl;
                text = this._GetRecentArticle_Html(url);
            }

            EncryptArgs encryp = new EncryptArgs();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject
                (
                new
                {
                    OfficialAccount = this._ResolveOfficialAccount(text, url),
                    Message = _ResolveBatchMessageFromJson(this._ExtracJson(text), encryp)
                }, Newtonsoft.Json.Formatting.Indented
                );
            return json;

        }




        /// <summary>
        /// 从（临时）文章页抽取正文部分
        /// </summary>
        /// <param name="url"></param>
        /// <param name="text"></param>
        /// <remarks>deal_article_content</remarks>
        /// <returns></returns>
        public string ExtractArticleMain(string url, string articlePageHtml = "")
        {

            if (!string.IsNullOrEmpty(articlePageHtml))
            {
                //pass
            }
            else if (!string.IsNullOrEmpty(url))
            {
                articlePageHtml = this._GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new Exception("deal_content need param url or text");
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(articlePageHtml);
            string bodyContent = "";
            try
            {
                bodyContent = doc.DocumentNode.SelectSingleNode("//*[@id='js_content']").InnerHtml;
            }
            catch (Exception e)
            { }

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
            return this._GetRelatedJson(url, title);

        }



        /// <summary>
        /// 请求文章相关评论（sogou此接口已经过期，待更新）
        /// </summary>
        /// <param name="text">文章页html</param>
        /// <param name="url">文章链接</param>
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
                articlePageHtml = this._GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new Exception("deal_content need param url or text");
            }


            Regex reg = new Regex("window.sg_data={(.*?)}");
            Match match = reg.Match(articlePageHtml);
            string sg_data = match.Groups[0].Value;
            sg_data = "{" + sg_data.Replace("\r\n", "").Replace(" ", "") + "}";

            reg = new Regex("{src:\"(.*?)\",ver:\"(.*?)\",timestamp:\"(.*?)\",signature:\"(.*?)\"}");
            var sgDataMatch = reg.Match(sg_data);
            string commentReqUrl = "http://mp.weixin.qq.com/mp/getcomment?src=" + sgDataMatch.Groups[0].Value + "&ver=" + sgDataMatch.Groups[1].Value +
                 "&timestamp=" + sgDataMatch.Groups[2] + "&signature=" + sgDataMatch.Groups[3].Value + "&uin=&key=&pass_ticket=&wxtoken=&devicetype=&clientversion=0&x5=0";

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("host", "mp.weixin.qq.com");
            headers.Add("referer", "http://mp.weixin.qq.com");
            HttpHelper netHelper = new HttpHelper();
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
                    logger.Error(errorMsg);
                    throw new WechatSogouException();
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
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
                articlePageHtml = this._GetOfficialAccountArticleHtml(url);
            }
            else
            {
                throw new Exception("dearlArticleOriginal need param url or text");
            }

            try
            {
                Regex reg = new Regex("var msg_link = \"(.*?)\";  ");
                Match match = reg.Match(articlePageHtml);
                originalLink = match.Groups[0].Value.Replace("amp;", "");
            }
            catch (Exception e)
            {
                if (articlePageHtml.Contains("系统出错"))
                {
                    logger.Debug("系统出错 - 链接问题，正常");
                }
                else if (articlePageHtml.Contains("此内容因违规无法查看"))
                {
                    logger.Debug("此内容因违规无法查看 - 剔除此类文章");
                }
                else
                {
                    logger.Error(e);
                    if (!String.IsNullOrEmpty(url))
                    {
                        logger.Error(url);
                    }
                    else
                    {
                        var reg = new Regex("<title>(.*?)</title>");
                        Match match = reg.Match(articlePageHtml);
                        if (match != null)
                        {
                            logger.Error(match.Groups[0].Value);
                        }
                        else
                        {
                            logger.Error(articlePageHtml);
                        }
                    }
                }
                throw new Exception();


            }


            return originalLink;


        }


        /// <summary>
        /// "获取微信搜狗搜索关键词联想
        /// </summary>
        /// <param name=""></param>
        /// <remarks>get_sugg</remarks>
        /// <returns>sugg: 联想关键词数组</returns>
        public string[] GetSuggestKeyWords(string keyWord)
        {
            string[] suggArray = new string[] { };
            string url = "http://w.sugg.sogou.com/sugg/ajaj_json.jsp?key=" + keyWord + "&type=wxpub&pr=web";
            HttpHelper netHelper = new HttpHelper();
            WebHeaderCollection headers = new WebHeaderCollection();
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
                logger.Error("sugg refind error", e);
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
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Host", "weixin.sogou.com");
            headers.Add("Referer", "http://weixin.sogou.com/");
            HttpHelper netHelper = new HttpHelper();
            string text = netHelper.Get(headers, url, "UTF-8");

            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            var targetArea = pageDoc.DocumentNode.SelectNodes("//*[@id='topwords']/li");
            List<HotWord> listTopWords = new List<HotWord>();

            foreach (var li in targetArea)
            {
                HotWord hotWord = new HotWord();
                try
                {
                    hotWord.Rank = Convert.ToInt16(li.SelectSingleNode("i").InnerText);
                    hotWord.Word = li.SelectSingleNode("a").InnerText;
                    hotWord.JumpLink = li.SelectSingleNode("a").GetAttributeValue("href", "");
                    hotWord.HotDegree = Convert.ToInt16(li.SelectSingleNode("span/span").GetAttributeValue("style", "").Replace("width:", "").Replace("%", "").Trim());
                    listTopWords.Add(hotWord);
                }
                catch (Exception e)
                {
                    logger.Debug(e);
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

            string pageStr = "";// "pc_" + page;
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
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Host", "weixin.sogou.com");
            headers.Add("Referer", "http://weixin.sogou.com/");
            headers.Add("Accept", "*/*");
            HttpHelper NetHelper = new HttpHelper();
            string text = NetHelper.Get(headers, url, "UTF-8");

            HtmlDocument pageDoc = new HtmlDocument();
            pageDoc.LoadHtml(text);
            string targetXpath = "";
            if (page == 0)
            {
                targetXpath = "//ul[@class='news-list']/li";
            }
            else
            {
                targetXpath = "li";
            }
            var targetArea = pageDoc.DocumentNode.SelectNodes(targetXpath);

            List<Article> ListArticle = new List<Article>();

            if (targetArea != null)
            {
                foreach (var li in targetArea)
                {
                    try
                    {

                        Article article = new Article() { Imgs = new List<string>() };
                        OfficialAccount account = new OfficialAccount();
                        article.Title = li.SelectSingleNode("div[2]/h3/a").InnerText;
                        article.Url = li.SelectSingleNode("div[1]/a").GetAttributeValue("href", "");
                        article.Brief = WebUtility.HtmlDecode(li.SelectSingleNode("div[2]/p[@class='txt-info']").InnerText);
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
                        if (isV == "1")
                        {
                            account.IsAuth = true;
                        }
                        else
                        {
                            account.IsAuth = false;
                        }
                        account.ProfilePicture = li.SelectSingleNode("div[2]/div/a").GetAttributeValue("data-headimage", "");


                        article.officialAccount = account;
                        ListArticle.Add(article);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                }

            }



            return ListArticle;

        }



        /// <summary>
        ///获取首页推荐文章公众号最近文章地址，所有分类，maxPage个页数
        /// </summary>
        /// <remarks>get_recent_article_url_by_index_all</remarks>
        /// <returns></returns>
        public List<Article> GetAllRecentArticle(int maxPage)
        {

            List<Article> listArticles = new List<Article>();

            for (int cateIndex = 0; cateIndex < 20; cateIndex++)
            {
                int pageIndex = 0;
                var articles = this.GetArticleByCategoryIndex(cateIndex, pageIndex);
                //while(!(articles == null))
                //
                while (pageIndex < maxPage)
                {
                    listArticles.AddRange(articles);
                    //listArticles.Add(urls.First().url);
                    pageIndex += 1;
                    articles = this.GetArticleByCategoryIndex(cateIndex, pageIndex);
                }
            }
            return listArticles;
        }





        private void deal_mass_send_msg_page(string wechatid, bool updatecache = true)
        {
            string url = "http://mp.weixin.qq.com/mp/getmasssendmsg?";
            // uin, key, biz, pass_ticket, frommsgid 
            //  var encry   = this._uinkeybiz("")


            //    try:
            //    session = self._cache_history_session(wechatid)
            //        r = session.get(url, headers ={ 'Host': 'mp.weixin.qq.com'}, verify = False)
            //    rdic = eval(r.text)
            //        if rdic['ret'] == 0:

            //        data_dict_from_str = self._str_to_dict(rdic['general_msg_list'])

            //        if rdic['is_continue'] == 0 and rdic['count'] == 0:
            //            raise WechatSogouEndException()

            //        msg_dict = self._deal_gzh_article_dict(data_dict_from_str)
            //            msg_dict_new = reversed(msg_dict)
            //            msgid = 0
            //            for m in msg_dict_new:
            //            if int(m['type']) == 49:
            //                msgid = m['qunfa_id']
            //                    break

            //        if updatecache:
            //            self._uinkeybiz(wechatid, rdic['uin_code'], rdic['key'], rdic['bizuin_code'], pass_ticket, msgid)

            //        return msg_dict
            //        else:
            //        logger.error('deal_mass_send_msg_page ret ' + str(rdic['ret']) + ' errmsg ' + rdic['errmsg'])
            //            raise WechatSogouHistoryMsgException(
            //                'deal_mass_send_msg_page ret ' + str(rdic['ret']) + ' errmsg ' + rdic['errmsg'])
            //except AttributeError:
            //    logger.error('deal_mass_send_msg_page error, please delete cache file')
            //        raise WechatSogouHistoryMsgException('deal_mass_send_msg_page error, please delete cache file')



        }






    }
}
