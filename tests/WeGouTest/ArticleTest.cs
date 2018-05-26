using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WeGouSharp;
using WeGouSharp.Model;
using Xunit;

namespace WeGouTest
{
    public class ArticletTest : BasicConfig
    {
        WeGou api;

        public ArticletTest()
        {
            api = new WeGou();
        }

        [Fact]
        public void TestSearchArticle()
        {
            var rs = api.SearchArticle("广州大学城");
            var articles = JsonConvert.DeserializeObject<List<Article>>(rs);
            Assert.True(rs.Length > 0);
        }
        
        [Fact]
        public void TestResolveArticleByUrl()
        {
            var rs = api.ResolveArticleByUrl("");
            var articles = JsonConvert.DeserializeObject<List<Article>>(rs);
            Assert.True(rs.Length > 0);
        }
        
        
        [Fact]
        public void TestResolveArticleByHtml()
        {
            var rs = api.ResolveArticleByHtml("");
            var articles = JsonConvert.DeserializeObject<List<Article>>(rs);
            Assert.True(rs.Length > 0);
        }
        
        
        [Fact]
        public void TestGetArticleByCategoryIndex()
        {
            var rs = api.GetArticleByCategoryIndex(1,2);
        }
        
        
        [Fact]
        public void TestGetAllRecentArticle()
        {
            var rs = api.GetAllRecentArticle(1);
        }
        
        
        
    }
}