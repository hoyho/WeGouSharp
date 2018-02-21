using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus
{

    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class Test
    {


        static public void run()
        {

            //创建实例
            WechatSogouAPI Sogou = new WechatSogouAPI();
            string jsonOutPut = "";


            // //搜索某关键字
            // var result = Sogou.SearchOfficialAccount("广州大学");
            // jsonOutPut = JsonConvert.SerializeObject(result,Formatting.Indented);
            // Console.Write(jsonOutPut);



            // //获取一个公号
            var AccountInfo= Sogou.GetAccountInfoById("ME_volunteer");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(AccountInfo, Newtonsoft.Json.Formatting.Indented);
            Console.Write(jsonOutPut);


            // //搜索公众号文章
            var articleList = Sogou.SearchArticle("广州大学城");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(articleList, Newtonsoft.Json.Formatting.Indented);
            Console.Write(jsonOutPut);


            // //公号的最近群发，参数不能同时为空，以下表示搜索id gzdxxmt
            var accountMessages = Sogou.GetOfficialAccountMessages("", "gzdxxmt", "");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(accountMessages, Newtonsoft.Json.Formatting.Indented);
            Console.Write(jsonOutPut);

            //获取公号以及群发消息
            var infoMsg = Sogou.GetOfficialAccountInfoAndMessages("", "", "广州大学");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(infoMsg, Newtonsoft.Json.Formatting.Indented);
            Console.Write(infoMsg);


            // //抽取文章正文
            var articleMain = Sogou.ExtractArticleMain("https://mp.weixin.qq.com/s?timestamp=1505141173&src=3&ver=1&signature=mjSrGDaCN1VXnicJAgNxoSq86-FiSBFQU*0UgI3MLPORXgGBKbPEvWwh3sZePZnfeK4lH59wa6SlqI97uuoDZRzIZr4G99vfrMO63vTgtSWGu6Oxa52I8pAZ4ZqzQbxPfM0yGWylLOlBXDJ7uWf*HM6pdD-H8Q79Oqg6jRkVRgM=", "");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(articleMain, Newtonsoft.Json.Formatting.Indented);
            Console.Write(jsonOutPut);


            // //获取联想词
            var suggest = Sogou.GetSuggestKeyWords("广州大学");
            jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(suggest, Newtonsoft.Json.Formatting.Indented);
            Console.Write(jsonOutPut);


            //获取首页热门
            //var hotSearch = Sogou.GetTopWords();
            //jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(hotSearch, Newtonsoft.Json.Formatting.Indented);
            //Console.Write(jsonOutPut);


            //获取首页其中一个主题分类的N页
            //var categoryArticle = Sogou.GetArticleByCategoryIndex(1,2);
            //jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(categoryArticle, Newtonsoft.Json.Formatting.Indented);
            //Console.Write(jsonOutPut);

            //获取首页全部分类的N页内容
            //var all = Sogou.GetAllRecentArticle(20);
            //jsonOutPut = Newtonsoft.Json.JsonConvert.SerializeObject(all, Newtonsoft.Json.Formatting.Indented);
            //Console.Write(jsonOutPut);


            Console.ReadKey();
        }
    }



}
