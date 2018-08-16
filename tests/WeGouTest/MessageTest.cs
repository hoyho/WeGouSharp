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
        readonly WeGouService _api;

        public MessageTest()
        {
            _api = ApiService;
        }

        [Fact]
        public void TestGetOfficialAccountMessagesById()
        {
            var rs = _api.GetOfficialAccountMessagesByIdAsync("bitsea").Result;
            Assert.True(rs != null && rs.FirstOrDefault().Author.Contains("和菜头"));
        }

        [Fact]
        public void TestGetOfficialAccountMessagesByIdSerializedAsync()
        {
            var rs = _api.GetOfficialAccountMessagesByIdSerializedAsync("prdcweixin").Result;
            Console.WriteLine(rs);
        }

        [Fact]
        public void TestGetOfficialAccountMessagesByName()
        {
            var test = _api.GetOfficialAccountMessagesByNameAsync("和菜头").Result;
            var msgs = JsonConvert.DeserializeObject<List<BatchMessage>>(test);
            Assert.True(test.Length > 0);
            Console.WriteLine(msgs);
        }


        [Fact]
        public void TestGetOfficialAccountMessagesBy()
        {
            //            var rs = _api.GetOfficialAccountMessagesByUrlAsync().Result;
            //            rs.Count>
            //todo
        }
    }
}