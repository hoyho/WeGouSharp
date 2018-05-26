using System;
using Newtonsoft.Json;
using Xunit;
using WeGouSharp;
using WeGouSharp.Model;

namespace WeGouTest
{
    public class AccountTest : BasicConfig
    {
//        WeGouService _api;

        public AccountTest()
        {
            //_api = new WeGouService();
        }

        [Fact]
        public void TestSearchOfficialAccount()
        {
            var test = ApiService.SearchOfficialAccount("广州大学");
            Assert.False(test.Count<0);
        }
        
        [Fact]
        public void TestGetAccountInfoById()
        {
            var rs = ApiService.GetAccountInfoById("ME_volunteer");
            Assert.False(rs.Name.Contains("广州大学"));
        }
        
    }
}