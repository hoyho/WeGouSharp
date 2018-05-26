using WeGouSharp;

namespace WeGouTest
{
    /// <summary>
    /// 基本配置，注入等初始化
    /// </summary>
    public class BasicConfig
    {
        public WeGou API;

        public BasicConfig()
        {
            API = new  WeGou();
        }

    }
}