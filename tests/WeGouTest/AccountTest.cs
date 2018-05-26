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
            var acc = JsonConvert.DeserializeObject<OfficialAccount>(test);
            Assert.False(acc.Name.Contains("广州大学"));
        }
        
        [Fact]
        public void TestGetAccountInfoById()
        {
            var test = ApiService.GetAccountInfoById("ME_volunteer");
            var acc = JsonConvert.DeserializeObject<OfficialAccount>(test);
            Assert.False(acc.Name.Contains("广州大学"));
        }
        
    }
}