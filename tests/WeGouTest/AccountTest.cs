using System;
using Newtonsoft.Json;
using Xunit;
using WeGouSharp;
using WeGouSharp.Model;

namespace WeGouTest
{
    public class AccountTest : BasicConfig
    {
        WeGou _api;

        public AccountTest()
        {
            _api = new WeGou();
        }

        [Fact]
        public void TestSearchOfficialAccount()
        {
            var test = _api.SearchOfficialAccount("广州大学");
            var acc = JsonConvert.DeserializeObject<OfficialAccount>(test);
            Assert.False(acc.Name.Contains("广州大学"));
        }
        
        [Fact]
        public void TestGetAccountInfoById()
        {
            var test = _api.GetAccountInfoById("ME_volunteer");
            var acc = JsonConvert.DeserializeObject<OfficialAccount>(test);
            Assert.False(acc.Name.Contains("广州大学"));
        }
        
    }
}