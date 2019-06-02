namespace WeGouSharp.Model
{
    public enum EngineType
    {
        Local, //install in your OS. Can be launched from any console
        
        Embed, //from embed path. for example, ./Resource/firefox_linux
        
        Remote //Remote driver. for example running a selenium or cluster at elsewhere which can be visit byhttp://10.252.90.122:4444/hub
    }
}