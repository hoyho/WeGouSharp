using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;
using WeGouSharp.Core;
using WeGouSharp.Model;

namespace WeGouSharp
{
    //暴露给外部调用的服务类
    public class WeGouService
    {
        readonly WechatSogouApi _wechatSogouApi;

        public WeGouService()
        {
            Program.EnsureInject();

            var logger = ServiceProviderAccessor.ResolveService<ILog>();
            var browser = ServiceProviderAccessor.ResolveService<Browser>();
            var conf = ServiceProviderAccessor.ResolveService<IConfiguration>();
            
            _wechatSogouApi = new WechatSogouApi(logger, browser, conf);
        }

        public WeGouService(ILog logger, Browser browser, IConfiguration conf)
        {
            _wechatSogouApi = new WechatSogouApi(logger, browser, conf);
        }

        #region 公众号

        //根据关键字搜索公众号
        public async Task<List<OfficialAccount>> SearchOfficialAccountAsync(string keyWord, int page = 1)
        {
            var accountList = await _wechatSogouApi.SearchOfficialAccountAsync(keyWord, page);
            return accountList;
        }

        //根据关键字搜索公众号(json)        
        public async Task<string> SearchOfficialAccountSerializedAsync(string keyWord, int page = 1)
        {
            var accountList = await _wechatSogouApi.SearchOfficialAccountAsync(keyWord, page);
            return Tools.TryParseJson(accountList);
        }


        //根据公众号id查询
        public async Task<OfficialAccount> GetAccountInfoByIdAsync(string accountId)
        {
            var account = await _wechatSogouApi.GetAccountInfoByIdAsync(accountId);
            return account;
        }

        //根据公众号id查询(json)
        public async Task<string> GetAccountInfoByIdSerializedAsync(string accountId)
        {
            var account = await _wechatSogouApi.GetAccountInfoByIdAsync(accountId);
            return Tools.TryParseJson(account);
        }

        #endregion


        #region 文章

        public async Task<string> SearchArticleAsync(string keyWord)
        {
            var article = await _wechatSogouApi.SearchArticleAsync(keyWord);
            return Tools.TryParseJson(article);
        }


        //从临时文章链接提取文章正文
        public async Task<string> ResolveArticleByUrl(string articleUrl)
        {
            var article = await _wechatSogouApi.ExtractArticleMain(articleUrl);
            return article;
        }


        //从临时文章页面的html代码中提取文章正文
        public async Task<string> ResolveArticleByHtml(string articleHtml)
        {
            var article = await _wechatSogouApi.ExtractArticleMain("", articleHtml);
            return article;
        }

        #endregion


        /// <summary>
        /// 通过账号的url链接获取信息
        /// </summary>
        /// <param name="accountPageUrl"></param>
        /// <returns></returns>
        public async Task<List<BatchMessage>> GetOfficialAccountMessagesByUrlAsync(string accountPageUrl = "")
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync(accountPageUrl);
            return rs;
        }

        /// <summary>
        /// 通过账号的url链接获取信息（json）
        /// </summary>
        /// <param name="accountPageUrl"></param>
        /// <returns></returns>
        public async Task<string> GetOfficialAccountMessagesByUrlSerializedAsync(string accountPageUrl = "")
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync(accountPageUrl);
            return Tools.TryParseJson(rs);
        }

        /// <summary>
        /// 通过账号的id获取信息
        /// </summary>
        /// <param name="wechatId"></param>
        /// <returns></returns>
        public async Task<List<BatchMessage>> GetOfficialAccountMessagesByIdAsync(string wechatId = "")
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync("", wechatId);
            return rs;
        }

        /// <summary>
        /// 通过账号的id获取信息(json)
        /// </summary>
        /// <param name="wechatId"></param>
        /// <returns></returns>
        public async Task<string> GetOfficialAccountMessagesByIdSerializedAsync(string wechatId = "")
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync("", wechatId);
            return Tools.TryParseJson(rs);
        }

        /// <summary>
        /// 通过账号的名称获取信息
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task<string> GetOfficialAccountMessagesByNameAsync(string accountName)
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync("", "", accountName);
            return Tools.TryParseJson(rs);
        }

        /// <summary>
        /// 通过账号的名称获取信息(json)
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task<string> GetOfficialAccountMessagesByNameSerializedAsync(string accountName)
        {
            var rs = await _wechatSogouApi.GetOfficialAccountMessagesAsync("", "", accountName);
            return Tools.TryParseJson(rs);
        }


        //获取联想词汇
        public async Task<string[]> GetSuggestKeyWordsAsync(string inputKeyWord)
        {
            var rs = await _wechatSogouApi.GetSuggestKeyWordsAsync(inputKeyWord);
            return rs;
        }

        //获取联想词汇
        public async Task<string> GetSuggestKeyWordsSerializedAsync(string inputKeyWord)
        {
            var rs = await _wechatSogouApi.GetSuggestKeyWordsAsync(inputKeyWord);
            return Tools.TryParseJson(rs);
        }

        //获取首页热搜词汇
        public async Task<List<HotWord>> GetTopWordsAsync()
        {
            var words = await _wechatSogouApi.GetTopWordsAsync();
            return words;
        }

        //获取首页热搜词汇
        public string GetTopWordsSerialized()
        {
            var words = _wechatSogouApi.GetTopWordsAsync();
            return Tools.TryParseJson(words);
        }


        public async Task<List<Article>> GetArticleByCategoryIndex(int categoryIndex, int page)
        {
            var rs = await _wechatSogouApi.GetArticleByCategoryIndex(categoryIndex, page);
            return rs;
        }

        public async Task<string> GetArticleByCategoryIndexSerialized(int categoryIndex, int page)
        {
            var rs = await _wechatSogouApi.GetArticleByCategoryIndex(categoryIndex, page);
            return Tools.TryParseJson(rs);
        }


        public async Task<List<Article>> GetAllRecentArticle(uint maxPage)
        {
            var articles = await _wechatSogouApi.GetAllRecentArticle((int) maxPage);
            return articles;
        }

        public string GetAllRecentArticleSerialized(uint maxPage)
        {
            var articles = _wechatSogouApi.GetAllRecentArticle((int) maxPage);
            return Tools.TryParseJson(articles);
        }

        //add more
    }
}