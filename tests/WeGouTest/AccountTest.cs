using System;
using Xunit;
using WeGouSharp;

namespace WeGouTest
{
    public class AccountTest
    {
        WeGou _api;

        public AccountTest(){
            _api = new WeGou();
        }

        [Fact]
        public void TestSearchOfficialAccount()
        {
            var test = _api.SearchOfficialAccount("");
            Assert.False(test==null);

        }
    }
}
