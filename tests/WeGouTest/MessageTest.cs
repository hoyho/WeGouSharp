using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using WeGouSharp;
using WeGouSharp.Model;

namespace WeGouTest
{
    public class MessageTest : BasicConfig
    {
        WeGouService _api;

        public MessageTest()
        {
            _api = ApiService;
        }

        [Fact]
        public void TestGetOfficialAccountMessagesById()
        {
            var rs = _api.GetOfficialAccountMessagesById("bitsea");
            Assert.False(rs.FirstOrDefault().Author.Contains("和菜头"));
        }
        
        [Fact]
        public void TestGetOfficialAccountMessagesByName()
        {
            var test = _api.GetOfficialAccountMessagesByName("和菜头");
            var msgs = JsonConvert.DeserializeObject<List<BatchMessage>>(test);

        }
        
        
        [Fact]
        public void TestGetOfficialAccountMessagesBy()
        {
            var rs = _api.GetOfficialAccountMessagesByUrl("");
            
            //todo

        }
        
    }
}