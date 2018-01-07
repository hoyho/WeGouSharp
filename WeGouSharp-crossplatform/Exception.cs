using System;
using System.Collections.Generic;
using System.Text;

namespace WeGouSharpPlus
{


    class WechatSogouException : Exception
    {
        //基于搜狗搜索的的微信公众号爬虫接口 异常基类
    }



    class WechatSogouVcodeException : WechatSogouException
    {

        public string MoreInfo
        {
            get;
            set;
        }
        ///基于搜狗搜索的的微信公众号爬虫接口 出现验证码 异常类
    }




    class WechatSogouVcodeOcrException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 验证码 识别错误 异常类
    }


    class WechatSogouJsonException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 非标准json数据 异常类
    }


    class WechatSogouEndException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 数据处理完成 异常类
    }



    class WechatSogouBreakException : WechatSogouException
    {
        //"基于搜狗搜索的的微信公众号爬虫接口 中断 异常类
    }


    class WechatSogouHistoryMsgException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 数据处理完成 异常类
    }



    class WeChatSogouConfigException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 配置错误 异常类
    }



    class WechatSogouRequestsException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 抓取 异常类
    }

}
