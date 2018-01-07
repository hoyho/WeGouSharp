using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus.Model
{
    class Article
    {
        public string Url { get; set; }

        public List<string> Imgs { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 文章简介
        /// </summary>
        public string Brief { get; set; }

        public string Time { get; set; }
        // public string officialAccount;

        public string ArticleListUrl { get; set; }//???

        public OfficialAccount officialAccount { get; set; }
    }
}
