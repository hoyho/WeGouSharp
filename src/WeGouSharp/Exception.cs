using System;

namespace WeGouSharp
{

//基于搜狗搜索的的微信公众号爬虫接口 异常基类
    class  WechatSogouException: Exception
    {
        public WechatSogouException(string info)
        {
            Info = info;
        }
        public WechatSogouException():this(String.Empty)
        {
        }
        
        public string Info;
        
    }
    
    
    



    //遇到搜狗的验证码
    class WechatSogouVcodeException : WechatSogouException
    {
        public WechatSogouVcodeException(string remark)
        {
            MoreInfo = remark;
        }

        public string MoreInfo
        {
            get;
            set;
        }
        public string VisittingUrl
        {
            get;
            set;
        }
    }
    
    
    //遇到微信的验证码
    class WechatWxVcodeException : WechatSogouException
    {
        public WechatWxVcodeException(string remark)
        {
            MoreInfo = remark;
        }

        public string MoreInfo
        {
            get;
            set;
        }
        public string VisittingUrl
        {
            get;
            set;
        }
    }

    ///验证码不通过
    class WechatSogouVcodeFailException : WechatSogouException
    {
        public WechatSogouVcodeFailException(string info)
        {
            Info = info;
        }
    }

    class WechatSogouVcodeOcrException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 验证码 识别错误 异常类
    }


    class WechatSogouJsonException : WechatSogouException
    {
        //基于搜狗搜索的的微信公众号爬虫接口 非标准json数据 异常类
    }


    //基于搜狗搜索的的微信公众号爬虫接口 文件 异常类
    class WechatSogouFileException : WechatSogouException
    {
        
    }


   //基于搜狗搜索的的微信公众号爬虫接口 request/response 异常类
    class WechatSogouRequestException : WechatSogouException
    {
        
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
