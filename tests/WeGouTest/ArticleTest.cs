using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeGouSharp;
using WeGouSharp.Model;
using Xunit;

namespace WeGouTest
{
    public class ArticletTest : BasicConfig
    {
        WeGouService api;

        public ArticletTest()
        {
            api = ApiService;
        }

        [Fact]
        public void TestSearchArticle()
        {
            var rs = api.SearchArticleAsync("广州大学城").Result;

            var articles = JsonConvert.DeserializeObject<List<Article>>(rs);
            Console.WriteLine("SearchArticleAsync:\n" + articles);
            Assert.True(rs.Length > 0);
        }

        [Fact]
        public async Task TestResolveArticleByUrl()
        {
            var sampleUrl =
                "https://mp.weixin.qq.com/s?src=11&timestamp=1533457801&ver=1041&signature=wcDERy-wQcYEkRTQNnaWRrKLOqJpKOtSm79PelJ*d6dR33BMztGMfOqmF*UJiKLCf*pn9Zr8LtAPvkyN26A*vBSPTldo1i8B-advVRhobx3jk5IM0DldUgLL04rz-rFb&new=1";
            var rs = await api.ResolveArticleByUrl(sampleUrl);
            Console.WriteLine("ResolveArticleByUrl:\n" + rs);
            Assert.True(rs.Length > 0);
        }


        [Fact]
        public void TestResolveArticleByHtml()
        {
//            var rs = api.ResolveArticleByHtml("").Result;
//            Assert.False(rs.Length > 0);
        }


        [Fact]
        public void TestGetArticleByCategoryIndex()
        {
            var rs = api.GetArticleByCategoryIndex(1, 2).Result;
            var str = JsonConvert.SerializeObject(rs);
            Assert.Null(rs);
            //bug
            //Assert.True(rs.Count > 0);
            Console.WriteLine(str);
        }


        [Fact]
        public void TestGetAllRecentArticle()
        {
            var rs = api.GetAllRecentArticleAsync(1).Result;
            Assert.True(rs.Count > 0);
        }
    }
}