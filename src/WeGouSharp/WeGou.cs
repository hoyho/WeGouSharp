namespace WeGouSharp
{
    //暴露给外部调用的上层类
    public class WeGou
    {
        WechatSogouAPI api = new WechatSogouAPI();
        public string SearchOfficialAccount(string keyWord, int page = 1)
        {
            var accountList = api.SearchOfficialAccount(keyWord, page);
            return Tools.TryParseJson(accountList);
        }
    }
}