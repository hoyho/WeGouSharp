using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WeGouSharp;
using WeGouSharp.Model;
using Xunit;

namespace WeGouTest
{
    public class WordsTest : BasicConfig
    {
        WeGouService api;

        public WordsTest()
        {
            api = ApiService;
        }

        [Fact]
        public void TestGetSuggestKeyWords()
        {
            var rs = api.GetSuggestKeyWordsAsync("广州大学城").Result;
            Assert.True(rs.Length > 0);
        }

        [Fact]
        public void TestGetTopWords()
        {
            var rs = api.GetTopWordsAsync().Result;
            Assert.True(rs.Count > 0);
        }


        [Fact]
        public void TestResolveArticleByHtml()
        {
            var rs = api.ResolveArticleByHtml("").Result;
            var articles = JsonConvert.DeserializeObject<List<Article>>(rs);
            Assert.True(rs.Length > 0);
            Console.WriteLine(articles);
        }
    }
}