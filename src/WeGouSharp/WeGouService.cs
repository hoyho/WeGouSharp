using System;
using System.Collections.Generic;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using WeGouSharp.Model;
using WeGouSharp.YunDaMa;

namespace WeGouSharp
{
    //暴露给外部调用的服务类
    public class WeGouService
    {

       private readonly WechatSogouAPI _wechatSogouApi = new WechatSogouAPI();

        /// <summary>
        /// 依赖服务注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="captchaDecode"></param>
        public WeGouService(ILog logger, IConfiguration configuration,IDecode captchaDecode)
        {
            //注入依赖服务
            var sp = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(logger)
                .AddSingleton(captchaDecode)
                .BuildServiceProvider();
            
            ServiceProviderAccessor.SetServiceProvider(sp);
            
        }
        

        #region 公众号
        //根据关键字搜索公众号
        public List<OfficialAccount> SearchOfficialAccount(string keyWord, int page = 1)
        {
            var accountList = _wechatSogouApi.SearchOfficialAccount(keyWord, page);
            return accountList;
        }
        
        //根据关键字搜索公众号(json)        
        public string SearchOfficialAccountSerialized(string keyWord, int page = 1)
        {
            var accountList = _wechatSogouApi.SearchOfficialAccount(keyWord, page);
            return Tools.TryParseJson(accountList);
        }


        //根据公众号id查询
        public OfficialAccount GetAccountInfoById(string accountId)
        {
            var account = _wechatSogouApi.GetAccountInfoById(accountId);
            return account;
        }
        
        //根据公众号id查询(json)
        public string GetAccountInfoByIdSerialized(string accountId)
        {
            var account = _wechatSogouApi.GetAccountInfoById(accountId);
            return Tools.TryParseJson(account);
        }

        #endregion


        #region 文章
        public string SearchArticle(string keyWord)
        {
            var article = _wechatSogouApi.SearchArticle(keyWord);
            return Tools.TryParseJson(article);
        }


        //从临时文章链接提取文章正文
        public string ResolveArticleByUrl(string articleUrl)
        {
            var article = _wechatSogouApi.ExtractArticleMain(articleUrl);
            return article;
        }


        //从临时文章页面的html代码中提取文章正文
        public string ResolveArticleByHtml(string articleHtml)
        {
            var article = _wechatSogouApi.ExtractArticleMain("", articleHtml);
            return article;
        }


        #endregion

//        public string GetOfficialAccountMessages(string accountPageUrl = "", string wechatId = "", string wechatName = "")
//        {
//            var rs = api.GetOfficialAccountMessages(accountPageUrl, wechatId, wechatName);
//            return Tools.TryParseJson(rs);
//        }
        
        /// <summary>
        /// 通过账号的url链接获取信息
        /// </summary>
        /// <param name="accountPageUrl"></param>
        /// <returns></returns>
        public List<BatchMessage> GetOfficialAccountMessagesByUrl(string accountPageUrl = "")
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages(accountPageUrl);
            return rs;
        }
       
        /// <summary>
        /// 通过账号的url链接获取信息（json）
        /// </summary>
        /// <param name="accountPageUrl"></param>
        /// <returns></returns>
        public string GetOfficialAccountMessagesByUrlSerialized(string accountPageUrl = "")
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages(accountPageUrl);
            return Tools.TryParseJson(rs);
        }
        
        /// <summary>
        /// 通过账号的id获取信息
        /// </summary>
        /// <param name="wechatId"></param>
        /// <returns></returns>
        public List<BatchMessage> GetOfficialAccountMessagesById(string wechatId = "")
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages("",wechatId,"");
            return rs;
        }
        
        /// <summary>
        /// 通过账号的id获取信息(json)
        /// </summary>
        /// <param name="wechatId"></param>
        /// <returns></returns>
        public string GetOfficialAccountMessagesByIdSerialized(string wechatId = "")
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages("",wechatId,"");
            return Tools.TryParseJson(rs);
        }
        
        /// <summary>
        /// 通过账号的名称获取信息
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public string GetOfficialAccountMessagesByName(string accountName)
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages("","",accountName);
            return Tools.TryParseJson(rs);
        }
        
        /// <summary>
        /// 通过账号的名称获取信息(json)
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public string GetOfficialAccountMessagesByNameSerialized(string accountName)
        {
            var rs = _wechatSogouApi.GetOfficialAccountMessages("","",accountName);
            return Tools.TryParseJson(rs);
        }
        

        //获取联想词汇
        public string[] GetSuggestKeyWords(string inputKeyWord)
        {
            var rs = _wechatSogouApi.GetSuggestKeyWords(inputKeyWord);
            return rs;
        }
        
        //获取联想词汇
        public string GetSuggestKeyWordsSerialized(string inputKeyWord)
        {
            var rs = _wechatSogouApi.GetSuggestKeyWords(inputKeyWord);
            return Tools.TryParseJson(rs);
        }

        //获取首页热搜词汇
        public List<HotWord> GetTopWords()
        {
            var words = _wechatSogouApi.GetTopWords();
            return words;
        }
        
        //获取首页热搜词汇
        public string GetTopWordsSerialized()
        {
            var words = _wechatSogouApi.GetTopWords();
            return Tools.TryParseJson(words);
        }
        

        public List<Article> GetArticleByCategoryIndex(int categoryIndex, int page)
        {
            var rs = _wechatSogouApi.GetArticleByCategoryIndex(categoryIndex, page);
            return rs;
        }
        
        public string GetArticleByCategoryIndexSerialized(int categoryIndex, int page)
        {
            var rs = _wechatSogouApi.GetArticleByCategoryIndex(categoryIndex, page);
            return Tools.TryParseJson(rs);
        }


        public List<Article> GetAllRecentArticle(uint maxPage)
        {
            var articles = _wechatSogouApi.GetAllRecentArticle((int)maxPage);
            return articles;
        }
        
        public string GetAllRecentArticleSerialized(uint maxPage)
        {
            var articles = _wechatSogouApi.GetAllRecentArticle((int)maxPage);
            return Tools.TryParseJson(articles);
        }

        //add more

    }


}