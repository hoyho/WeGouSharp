using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus.Model
{
   public class BatchMessage
    {
        public int Meaasgeid { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string SendDate { get; set; } //

        public string Type { get; set; } //49:图文，1:文字，3:图片，34:音频，62:视频

        public string Content { get; set; } // for type 1

        public string ImageUrl { get; set; } //for type 49

        public string PlayLength { get; set; }// for type 音频

        public int FileId { get; set; }// for type 音频 or 图文

        public string AudioSrc { get; set; } // for type 音频

        //for type 图文
        public string ContentUrl { get; set; }

        public int Main { get; set; }

        public string Title { get; set; }

        public string Digest { get; set; }

        public string SourceUrl { get; set; }

        public string Cover { get; set; }

        public string Author { get; set; }

        public string CopyrightStat { get; set; }

        //for type 视频
        public string CdnVideoId { get; set; }

        public string Thumb { get; set; }

        public string VideoSrc { get; set; }

        //others
    }
}
