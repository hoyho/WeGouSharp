using Xunit;

namespace WeGouTest
{
    public class AccountTest : BasicConfig
    {

        [Fact]
        public void TestSearchOfficialAccount()
        {
            var test = ApiService.SearchOfficialAccountAsync("广州大学").Result;
            Assert.False(test.Count<0);
        }
        
        [Fact]
        public void TestGetAccountInfoById()
        {
            var rs = ApiService.GetAccountInfoByIdAsync("ME_volunteer").Result;
            Assert.False(rs.Name.Contains("广州大学"));
        }
        
    }
}