# WeGouSharp
基于搜狗的微信公众号定向爬虫，使用C#实现，故取名WeGouSharp

关于微信公共号爬虫的项目网上已经不少，然而基本大多数的都是使用Python实现
鉴于鄙人是名.NET开发人员，于是又为广大微软系同胞创建了这个轮子，使用C#实现的微信爬虫
蓝本为[Chyroc/WechatSogou](https://github.com/Chyroc/WechatSogou)
在此还请各位大佬指教

# 使用

### 初始化 API
在test中直接调用

```C#
            //创建实例
            WechatSogouApi Sogou = new WechatSogouApi();

            var result = Sogou.SearchOfficialAccount("广州大学");

            //var result = Sogou.GetOfficialAccountMessages("","bitsea",""); // get_gzh_message
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Console.Write(json);
            Console.ReadKey();
```
运行截图：
![运行结果](https://github.com/hoyho/WeGouSharp/blob/master/ScreenShot/SearchOfficialAccount.png?raw=true)

