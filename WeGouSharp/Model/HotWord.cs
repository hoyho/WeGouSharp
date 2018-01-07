using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus.Model
{
    /// <summary>
    /// 首页搜索热词
    /// </summary>
    class HotWord
    {
        /// <summary>
        /// 排行
        /// </summary>
        public int Rank { get; set; }

        public string Word { get; set; }

        public string JumpLink { get; set; }

        /// <summary>
        /// 热度
        /// </summary>
        public int HotDegree { get; set; } 
    }
}
