namespace WeGouSharp
{
    //暴露给外部调用的上层类
    public class WeGou
    {
        WechatSogouAPI api = new WechatSogouAPI();


        #region 公众号
        //根据关键字搜索公众号
        public string SearchOfficialAccount(string keyWord, int page = 1)
        {
            var accountList = api.SearchOfficialAccount(keyWord, page);
            return Tools.TryParseJson(accountList);
        }


        //根据公众号id查询
        public string GetAccountInfoById(string accountId)
        {
            var account = api.GetAccountInfoById(accountId);
            return Tools.TryParseJson(account);
        }

        #endregion


        #region 文章
        public string SearchArticle(string keyWord)
        {
            var article = api.SearchArticle(keyWord);
            return Tools.TryParseJson(article);
        }


        //从临时文章链接提取文章正文
        public string ResolveArticleByUrl(string articleUrl)
        {
            var article = api.ExtractArticleMain(articleUrl);
            return article;
        }


        //从临时文章页面的html代码中提取文章正文
        public string ResolveArticleByHtml(string articleHtml)
        {
            var article = api.ExtractArticleMain("", articleHtml);
            return article;
        }


        #endregion

        public string GetOfficialAccountMessages(string accountPageUrl = "", string wechatId = "", string wechatName = "")
        {
            var rs = api.GetOfficialAccountMessages(accountPageUrl, wechatId, wechatName);
            return Tools.TryParseJson(rs);
        }

        //获取联想词汇
        public string GetSuggestKeyWords(string inputKeyWord)
        {
            var rs = api.GetSuggestKeyWords(inputKeyWord);
            return Tools.TryParseJson(rs);
        }

        //获取首页热搜词汇
        public string GetTopWords()
        {
            var words = api.GetTopWords();
            return Tools.TryParseJson(words);
        }

        public string GetArticleByCategoryIndex(int categoryIndex, int page)
        {
            var rs = api.GetArticleByCategoryIndex(categoryIndex, page);
            return Tools.TryParseJson(rs);
        }


        public string GetAllRecentArticle(uint maxPage)
        {
            var articles = api.GetAllRecentArticle((int)maxPage);
            return Tools.TryParseJson(articles);
        }

        //add more

    }


}